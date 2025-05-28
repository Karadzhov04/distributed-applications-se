namespace ClickEatApp.Web.ImageProcess
{
    public class PhotoProcess
    {
        public static string CreateImage(IFormFile Image)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(stream);
            }

            return $"/images/{fileName}";
        }
    }
}
