﻿namespace ProductApi.Models
{
    public class UserProfileProduct
    {
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
