# Fast_R_CNN_Object_detection
The code uses the TensorFlow Hub library to load a pre-trained object detection model from the specified module_handle. The model used in this code is the Faster R-CNN with Inception ResNet V2 architecture trained on the COCO dataset.

![alt text] (https://github.com/radubm1/Fast_R_CNN_Object_detection/blob/main/image.png)

| Type                            | Deffinition                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
|---------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Static Private Member Functions | Static Private Member Functions                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| static void                     | Main (string[] args)                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
|                                 | This function reads an image file from the given path and returns the decoded image as a TensorFlow tensor.                                                                                                                                                                                                                                                                                                                                                               |
| static string                   | DownloadAndResizeImage (string url, int new_width=256, int new_height=256, bool display=true)                                                                                                                                                                                                                                                                                                                                                                             |
|                                 | This function downloads an image from the given URL, resizes it to the specified dimensions, and saves it as a JPEG file. It returns the path of the downloaded image. If the display parameter is set to True, it also displays the downloaded image using the save_image() function.                                                                                                                                                                                    |
| static Image                    | ResizeImage (Image image, int new_width, int new_height)                                                                                                                                                                                                                                                                                                                                                                                                                  |
| static void                     | SaveImage (string filename)                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| static void                     | DrawBoundingBoxOnImage (Image image, float ymin, float xmin, float ymax, float xmax, Color color, Font font, int thickness=4, List< string > display_str_list=null)                                                                                                                                                                                                                                                                                                       |
|                                 | This function draws a bounding box on the given image using the provided coordinates and color. It also adds labels to the bounding box. The font parameter specifies the font style and size for the labels.                                                                                                                                                                                                                                                             |
| static Image                    | DrawBoxes (Image image, float[,] boxes, string[] class_names, float[] scores, int max_boxes=10, float min_score=0.1f)                                                                                                                                                                                                                                                                                                                                                     |
|                                 | This function draws bounding boxes on the image for detected objects. It takes the image, bounding box coordinates, class names, and scores as input. It uses the draw_bounding_box_on_image() function to draw the boxes and labels. The max_boxes parameter limits the number of boxes to be drawn, and the min_score parameter filters out boxes with scores below the specified threshold.                                                                            |
| static void                     | RunDetector (dynamic detector, string path)                                                                                                                                                                                                                                                                                                                                                                                                                               |
|                                 | This function runs the object detection model on the image specified by the path parameter. It loads the image using the load_img() function, converts it to the required format, and passes it to the detector model. The result is a dictionary containing the detection boxes, class entities, and scores. The function then calls the draw_boxes() function to draw the bounding boxes on the image and displays the final result using the display_image() function. |
