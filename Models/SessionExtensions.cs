// Importing necessary namespaces and libraries
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

// Namespace for the data models of the application
namespace AmazonCloneMVC.Models
{
    // Extension class for working with session storage
    public static class SessionExtensions
    {
        // Extension method to store a CartService object in session
        public static void SetCartService(this ISession session, CartService cartService)
        {
            // Serialize the CartService object to JSON
            string cartServiceJson = JsonConvert.SerializeObject(cartService);
            // Store the JSON representation in the session with the key "CartService"
            session.SetString("CartService", cartServiceJson);
        }

        // Extension method to retrieve a CartService object from session
        public static CartService GetCartService(this ISession session)
        {
            // Retrieve the JSON representation of CartService from session
            string cartServiceJson = session.GetString("CartService");
            // Check if the JSON representation is not null
            if (cartServiceJson != null)
            {
                // Deserialize the JSON representation to a CartService object and return it
                return JsonConvert.DeserializeObject<CartService>(cartServiceJson);
            }
            // Return a new instance of CartService if the object is not found in session
            return new CartService();
        }
    }
}
