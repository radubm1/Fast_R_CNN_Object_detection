using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.NumPy;


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
    static string DownloadAndResizeImage(string url, int new_width = 256, int new_height = 256, bool display = true)
    {
        var filename = Path.GetTempFileName() + ".jpg";
        var client = new WebClient();
        client.Headers.Add("user-agent", "Only a test!");
        var image_data = client.DownloadData(url);
        var ms = new MemoryStream(image_data);
        var image = Image.FromStream(ms);
        var resized_image = ResizeImage(image, new_width, new_height);
        resized_image.Save(filename, ImageFormat.Jpeg);
        Console.WriteLine("The image has been downloaded and saved at: {0}.", filename);
        return filename;
    }
    static Image ResizeImage(Image image, int new_width, int new_height)
    {
        var resized_image = new Bitmap(new_width, new_height);
        var graphics = Graphics.FromImage(resized_image);
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.DrawImage(image, 0, 0, new_width, new_height);
        return resized_image;
    }
    static void SaveImage(string filename)
    {
        var image = Image.FromFile(filename);
        var bitmap = new Bitmap(image);
        bitmap.SetResolution(96, 96);
        bitmap.Save("temp.bmp", ImageFormat.Bmp);
    }
    static void DrawBoundingBoxOnImage(Image image, float ymin, float xmin, float ymax, float xmax, Color color, Font font, int thickness = 4, List<string> display_str_list = null)
    {
        var graphics = Graphics.FromImage(image);
        var im_width = image.Width;
        var im_height = image.Height;
        var left = (int)(xmin * im_width);
        var right = (int)(xmax * im_width);
        var top = (int)(ymin * im_height);
        var bottom = (int)(ymax * im_height);
        var pen = new Pen(color, thickness);
        graphics.DrawRectangle(pen, left, top, right - left, bottom - top);
        if (display_str_list != null)
        {
            var total_display_str_height = (int)(1 + 2 * 0.05) * display_str_list.Count;
            var text_bottom = top > total_display_str_height ? top : top + total_display_str_height;
            for (var i = display_str_list.Count - 1; i >= 0; i--)
            {
                var display_str = display_str_list[i];
                var text_size = graphics.MeasureString(display_str, font);
                var text_width = (int)text_size.Width;
                var text_height = (int)text_size.Height;
                var margin = (int)Math.Ceiling(0.05 * text_height);
                graphics.FillRectangle(Brushes.White, left, text_bottom - text_height - 2 * margin, text_width, text_bottom);
                graphics.DrawString(display_str, font, Brushes.Black, new PointF(left + margin, text_bottom - text_height - margin));
                text_bottom -= text_height - 2 * margin;
            }
        }
    }
    static Image DrawBoxes(Image image, float[,] boxes, string[] class_names, float[] scores, int max_boxes = 10, float min_score = 0.1f)
    {
        var colors = typeof(Color).GetProperties().Where(p => p.PropertyType == typeof(Color))
    .Select(p => (Color)p.GetValue(null)).ToArray();
        var font = SystemFonts.DefaultFont;
        try
        {
            font = new Font(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "/Microsoft/Windows/Fonts/Dance Floor.ttf", 10);
        }
        catch (IOException)
        {
            Console.WriteLine("The required font was not found. Using the default font.");
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
    static void RunDetector(dynamic detector, string path)
    {
        var image = Image.FromFile(path);
        var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Jpeg);
        var image_data = ms.ToArray();
        dynamic converted_img = tf.convert_to_tensor(image_data, tf.float32);
        var result = detector(converted_img);
        var result_dict = new Dictionary<string, dynamic>();
        foreach (KeyValuePair<string, dynamic> kvp in result) result_dict[kvp.Key] = kvp.Value.numpy();
        DrawBoxes(image, result["detection_boxes"], result["detection_class_entities"], result["detection_scores"]);
        Console.WriteLine("Found {0} objects.", result_dict);
    }
}
