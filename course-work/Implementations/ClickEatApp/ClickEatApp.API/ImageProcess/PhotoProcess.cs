namespace ClickEatApp.API.ImageProcess
{
    public class PhotoProcess
    {
        public static string CreateImage(IFormFile Image, string type)
        {
            var folder = type switch
            {
                "restaurant" => "Restaurant",
                "food" => "Food",
                _ => "images/others"
            };

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);

            // Път до wwwroot/images на ClickEatApp.Web проекта
            var webRootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "ClickEatApp.Web", "wwwroot", "images", $"{folder}");

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            string filePath = Path.Combine(webRootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(stream);
            }
            return $"/images/{folder}/{fileName}";
        }

    }
}
