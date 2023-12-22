using AmazonCloneMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AmazonCloneMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;

        // Constructeur avec injection de dépendances
        public HomeController(ILogger<HomeController> logger, MyDbContext context, IMemoryCache memoryCache)
        {
            _logger = logger;
            _context = context;
            _cache = memoryCache;
        }

        // Action pour la page d'accueil
        public async Task<IActionResult> Index([Bind("searchString")] string searchString)
        {
            IEnumerable<Produit> myDbContext;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Utilisation du cache pour les résultats de recherche
                myDbContext = await _cache.GetOrCreateAsync($"SearchResults_{searchString}", async entry =>
                {
                    // Logique de recherche
                    _logger.LogInformation("Searching a product that contains the string: {searchString}", searchString);

                    var results = await _context.Produits.Include(p => p.Categorie)
                                                           .Where(p => p.Description.Contains(searchString))
                                                           .ToListAsync();

                    // Durée d'expiration du cache
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return results;
                });
            }
            else
            {
                // Utilisation du cache pour tous les produits
                myDbContext = await _cache.GetOrCreateAsync("AllProducts", async entry =>
                {
                    // Logique pour tous les produits
                    _logger.LogInformation("Home page visited at {DT}", DateTime.UtcNow.ToLongTimeString());

                    var results = await _context.Produits.Include(p => p.Categorie).ToListAsync();

                    // Durée d'expiration du cache
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return results;
                });
            }

            return View(myDbContext);
        }

        // Action pour ajouter un produit au panier
        public async Task<IActionResult> AddToCard([Bind("ProduitId")] int ProduitId)
        {
            var product = await _context.Produits.FindAsync(ProduitId);

            if (product != null)
            {
                // Logique pour ajouter un produit au panier
                _logger.LogInformation("Product added to cart at: {DT}", DateTime.UtcNow.ToLongTimeString());

                var retrievedCartService = HttpContext.Session.GetCartService();
                retrievedCartService._cartItems.Add(product);
                HttpContext.Session.SetCartService(retrievedCartService);
            }
            else
            {
                // Logique si le produit est null
                _logger.LogWarning("NULL Product cannot be added: {DT}", DateTime.UtcNow.ToLongTimeString());
            }

            return RedirectToAction(nameof(Index));
        }

        // Action pour afficher le panier
        public IEnumerable<Produit> CartProduit;
        public IActionResult Cart()
        {
            var retrievedCartService = HttpContext.Session.GetCartService();
            if (retrievedCartService != null)
            {
                CartProduit = retrievedCartService._cartItems;
            }
            return View(CartProduit);
        }

        // Action pour supprimer un produit du panier
        public IActionResult RemoveProd([Bind("prodId")] int prodId)
        {
            var retrievedCartService = HttpContext.Session.GetCartService();
            if (retrievedCartService != null)
            {
                // Logique pour supprimer un produit du panier
                retrievedCartService._cartItems.Remove(retrievedCartService._cartItems.Find(pro => pro.ProduitID == prodId));
                HttpContext.Session.SetCartService(retrievedCartService);
            }
            return RedirectToAction(nameof(Index));
        }

        // Action pour la page de confidentialité
        public IActionResult Privacy()
        {
            return View();
        }

        // Action pour gérer les erreurs
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
