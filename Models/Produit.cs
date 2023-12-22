using System.ComponentModel.DataAnnotations.Schema;

namespace AmazonCloneMVC.Models
{
    [Table("PRODUITS")]
    public class Produit
    {

        public int ProduitID { get; set; }
        public string ProduitName { get; set; }

        public string Description { get; set; }
        public double Prix { get; set; }
        public int Quantite { get; set; }
        public string ImagePath { get; set; }
        public int CategorieID { get; set; }
        [ForeignKey("CategorieID")]
        public virtual Categorie? Categorie { get; set; }
    }
}
