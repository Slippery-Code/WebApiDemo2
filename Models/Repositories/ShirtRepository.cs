namespace WebApiDemo2.Models.Repositories
{
    public static class ShirtRepository //otan hostaroume ta data to kanoume static class
    {
        //in memory list
        private static List<Shirt> shirts = new List<Shirt>()
        {
            new Shirt {ShirtId=1, Brand="My Brand", Color="Blue", Gender="Men", Price =30, Size = 10},
            new Shirt {ShirtId=2, Brand="My Brand", Color="Blue", Gender="Men", Price =35, Size = 12},
            new Shirt {ShirtId=3, Brand="My Brand", Color="Blue", Gender="Women", Price =28, Size = 8},
            new Shirt {ShirtId=4, Brand="My Brand", Color="Blue", Gender="Women", Price =30, Size = 9}
        };

        public static List<Shirt> GetShirts()
        {
            return shirts;
        }

        public static bool ShirtExists(int id)
        {
            return shirts.Any(x => x.ShirtId == id);
        }
        /*otan sou bgazei prasini gramoula san error apo katw shmainei oti exeis orisei
                                                  kapou oti mporei na einai nullable an son typo epistrofis prostheseis ? feugei entelos giati etsi
                                                  h epistrofi mporei na einai kai nullable */
        public static Shirt? GetShirtById(int id) 
        {
            return shirts.FirstOrDefault(x => x.ShirtId == id);
        }


        public static Shirt? GetShirtByProperties(string? brand,string? gender, string? color, int? size)
        {
            //xrisimopoiw erotimatiko se ola giati xrisimopoioume kai sthn klasi tou shirt //to kanoume to parakatw gia na mhn exoume do diaforetika antikeimena me tis idies times
            return shirts.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(brand)
            && !string.IsNullOrWhiteSpace(x.Brand) &&
            x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(gender) &&
            !string.IsNullOrWhiteSpace(x.Gender) &&
            x.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(color) &&
            !string.IsNullOrWhiteSpace(x.Color) &&
            x.Color.Equals(color, StringComparison.OrdinalIgnoreCase) &&
            size.HasValue &&
            x.Size.HasValue &&
            size.Value == x.Size.Value);
        }

        public static void AddShirt(Shirt shirt) //na prosthesoume shirt sthn lista
        {
            //to antikeimeno den prosthei shirt Id
            //epeidh den exoume kapoio database akomi prepei na to kanoume automata
            int maxId = shirts.Max(x => x.ShirtId); //auto mas dinei to max id
            shirt.ShirtId = maxId + 1; 
            shirts.Add(shirt);
        }

        public static void UpdateShirt(Shirt shirt)
        {
            var shirtToUpdate = shirts.First(x => x.ShirtId == shirt.ShirtId);
            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Price = shirt.Price;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.Color = shirt.Color;
            shirtToUpdate.Gender = shirt.Gender;
            
        }

        public static void DeleteShirt(int shirtId)
        {
            var shirt = GetShirtById(shirtId);
            if (shirt!=null) 
            {
                shirts.Remove(shirt); //hard delete
            }
        }
    }
}
