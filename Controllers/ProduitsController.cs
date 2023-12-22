using AmazonCloneMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmazonCloneMVC.Controllers
{
    [Authorize]
    public class ProduitsController : Controller
    {
        private readonly MyDbContext _context;

        public ProduitsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Produits
        public async Task<IActionResult> Index()
        {
            // Retrieve all products including their categories
            var myDbContext = _context.Produits.Include(p => p.Categorie);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Produits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            // Retrieve product details including its category
            var produit = await _context.Produits
                .Include(p => p.Categorie)
                .FirstOrDefaultAsync(m => m.ProduitID == id);

            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // GET: Produits/Create
        public IActionResult Create()
        {
            // Populate the category dropdown list for product creation
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "NomCategorie");

            return View();
        }

        // POST: Produits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProduitID,ProduitName,Description,Prix,Quantite,ImagePath,CategorieID")] Produit produit, IFormFile file)
        {
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                // Check if an image file is provided
                if (file != null)
                {
                    // Save the image to a directory on the server
                    var fileName = Path.GetFileName(file.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update the product's image path in the database
                    produit.ImagePath = "images/" + fileName;
                }

                // Add the product to the database and save changes
                _context.Add(produit);
                await _context.SaveChangesAsync();

                // Redirect to the index page
                return RedirectToAction(nameof(Index));
            }

            // Repopulate the category dropdown list if the model is not valid
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "NomCategorie");

            return View(produit);
        }

        // GET: Produits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            // Retrieve the product for editing
            var produit = await _context.Produits.FindAsync(id);

            if (produit == null)
            {
                return NotFound();
            }

            // Populate the category dropdown list for product editing
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "NomCategorie", produit.CategorieID);

            return View(produit);
        }

        // POST: Produits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProduitID,ProduitName,Description,Prix,Quantite,ImagePath,CategorieID")] Produit produit)
        {
            if (id != produit.ProduitID)
            {
                return NotFound();
            }

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Update the product in the database and save changes
                    _context.Update(produit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exceptions
                    if (!ProduitExists(produit.ProduitID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the index page
                return RedirectToAction(nameof(Index));
            }

            // Repopulate the category dropdown list if the model is not valid
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "NomCategorie", produit.CategorieID);

            return View(produit);
        }

        // GET: Produits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            // Retrieve the product for deletion
            var produit = await _context.Produits
                .Include(p => p.Categorie)
                .FirstOrDefaultAsync(m => m.ProduitID == id);

            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: Produits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Produits == null)
            {
                return Problem("Entity set 'MyDbContext.Produits' is null.");
            }

            // Retrieve and remove the product from the database
            var produit = await _context.Produits.FindAsync(id);

            if (produit != null)
            {
                _context.Produits.Remove(produit);
            }

            await _context.SaveChangesAsync();

            // Redirect to the index page
            return RedirectToAction(nameof(Index));
        }

        // Check if a product with a given ID exists
        private bool ProduitExists(int id)
        {
            return (_context.Produits?.Any(e => e.ProduitID == id)).GetValueOrDefault();
        }
    }
}
