using System;

public class Program
{
    public static void Main(string[] args)
    {
        var image_url = "https://upload.wikimedia.org/wikipedia/commons/0/09/Fruit_and_vegetables_basket.jpg";
        var downloaded_image_path = DownloadAndResizeImage(image_url, 1280, 856);
        SaveImage(downloaded_image_path);
        var module_handle = "C:\\models\\";
        var detector = tf.saved_model.load(module_handle);
        RunDetector(detector, downloaded_image_path);
    }
    static string DownloadAndResizeImage (string  url, int  new_width = 256, int  new_height = 256, bool  display = true)
    {
        var filename = Path.GetTempFileName() + ".jpg";
        var client = new WebClient()
        client.Headers.Add("user-agent", "Only a test!");
        var image_data = client.DownloadData(url);
        var ms = new MemoryStream(image_data))
        var image = Image.FromStream(ms))
        var resized_image = ResizeImage(image, new_width, new_height))
        resized_image.Save(filename, ImageFormat.Jpeg);
        Console.WriteLine("The image has been downloaded and saved at: {0}.", filename);
        return filename;
    }
    static Image ResizeImage (Image image, int new_width, int new_height)
    {
        var resized_image = new Bitmap(new_width, new_height);
        var graphics = Graphics.FromImage(resized_image)
        graphics.CompositingQuality = CompositingQuali.HighQualit;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.DrawImage(image, 0, 0, new_width, new_height);
        return resized_image;
    }
    static void SaveImage (string filename)
    {
        var image = Image.FromFile(filename))
        var bitmap = new Bitmap(image))
        bitmap.SetResolution(96, 96);
        bitmap.Save("temp.bmp", ImageFormat.Bmp);
    }
    static void DrawBoundingBoxOnImage (Image image, float ymin, float xmin, float ymax, float xmax, Color color, Font font, int thickness=4, List< string > display_str_list=null)
    {
        var colors = typeof(Color).GetProperties().Where(p => p.PropertyType == typeof(Color))
            .Select(p => (Color)p.GetValue(null)).ToArray();
            Font font = null;
            try
            {
                font = new Font(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "/Microsoft/Windows/Fonts/Dance Floor.ttf", 10);
            }
            catch (IOException)
            {
                Console.WriteLine("The required font was not found. Using the default font.");
                font = SystemFonts.DefaultFont;
            }
            for (var i = 0; i < Math.Min(boxes.GetLength(0), max_boxes); i++)
                if (scores[i] >= min_score)
                    {
                        var ymin = boxes[i, 0];
                        var xmin = boxes[i, 1];
                        var ymax = boxes[i, 2];
                        var xmax = boxes[i, 3];
                        var display_str = $"{class_names[i]}: {Math.Round(scores[i] * 100)}%";
                        var color = colors[Math.Abs(class_names[i].GetHashCode()) % colors.Length];
                        DrawBoundingBoxOnImage(image, ymin, xmin, ymax, xmax, color, font, display_str_list: new List<string> { display_str });
                    }
        return image;
    }
}
