 import os
 import tempfile
 from urllib.request import urlopen
 from io import BytesIO
 from PIL import Image
 from PIL import ImageOps
 from PIL import ImageColor
 from PIL import ImageDraw
 from PIL import ImageFont
 import matplotlib.pyplot as plt
 import tensorflow_hub as hub
 import tensorflow as tf
 import time
 import numpy as np
 def display_image(image):
 fig = plt.figure(figsize=(20, 15))
 plt.grid(False)
 plt.imshow(image)
 plt.show()
 def download_and_resize_image(url, new_width=256,
 new_height=256,display=True):
 _, filename = tempfile.mkstemp(suffix=".jpg")
 response = urlopen(url)
 image_data = response.read()
 image_data = BytesIO(image_data)
 pil_image = Image.open(image_data)
 pil_image = ImageOps.fit(pil_image,
 (new_width, new_height), Image.LANCZOS)
 pil_image_rgb = pil_image.convert("RGB")
 pil_image_rgb.save(filename, format="JPEG", quality=90)
 print("The image was downloaded here %s." % filename)
 if display:
 display_image(pil_image)
 return filename
 def draw_bounding_box_on_image(image, ymin, xmin, ymax, xmax,
 color,font,thickness=4,display_str_list=()):
 # Add rectangular marks:
 draw = ImageDraw.Draw(image)
 im_width, im_height = image.size
 (left, right, top, bottom) = (xmin * im_width, xmax * im_width,
 ymin * im_height, ymax * im_height)
 draw.line([(left, top), (left, bottom), (right, bottom), (right, top),
 (left, top)], width=thickness,fill=color)
 # For rectangles whose dimensions surpass the initial image, labels go to bottom
 display_str_heights = [font.getbbox(ds)[3] for ds in display_str_list]
 # Each line has a top and bottom spacing of 0.05x.
 total_display_str_height = (1 + 2 * 0.05) * sum(display_str_heights)
 if top > total_display_str_height:
 text_bottom = top
 else:
 text_bottom = top + total_display_str_height
 # Reverse the list and print from end to start:
 for display_str in display_str_list[::-1]:
 bbox = font.getbbox(display_str)
 text_width, text_height = bbox[2], bbox[3]
 margin = np.ceil(0.05 * text_height)
 draw.rectangle([(left, text_bottom- text_height- 2 * margin),
 (left + text_width, text_bottom)],
 fill=color)
 draw.text((left + margin, text_bottom- text_height- margin),
 display_str,
 fill="black",
 font=font)
 text_bottom-= text_height- 2 * margin
 def draw_boxes(image, boxes, class_names, scores, max_boxes=10, min_score=0.1):
 # Draw rectangles with the corresponding labels and percentages:
 colors = list(ImageColor.colormap.values())
 try:
 font = ImageFont.truetype(os.environ['LOCALAPPDATA'] +\
 "/Microsoft/Windows/Fonts/DanceFloor.ttf", 10)
 except IOError:
 print("I did not find the required font, I am using the default font!")
 font = ImageFont.load_default()
 for i in range(min(boxes.shape[0], max_boxes)):
 if scores[i] >= min_score:
 ymin, xmin, ymax, xmax = tuple(boxes[i])
 display_str = "{}: {}%".format(class_names[i].decode("ascii"), int(100 * scores[i]))
 color = colors[hash(class_names[i]) % len(colors)]
 image_pil = Image.fromarray(np.uint8(image)).convert("RGB")
 draw_bounding_box_on_image(image_pil, ymin, xmin, ymax, xmax, color, font,
 display_str_list=[display_str])
 np.copyto(image, np.array(image_pil))
 return image
 image_url="https://upload.wikimedia.org/wikipedia/commons/5/5b/Cebras_de_Burchell_%28Eq\
 uus_quagga_burchellii%29%2C_vista_a%C3%A9rea_del_delta_del_Okavango%2C_Botsu\
 ana%2C_2018-08-01%2C_DD_30.jpg"
 downloaded_image_path = download_and_resize_image(image_url, 1280, 856, True)
 module_handle = "https://tfhub.dev/google/faster_rcnn/openimages_v4/inception_resnet_v2/1"
 detector = hub.load(module_handle).signatures['default']
 def load_img(path):
 img = tf.io.read_file(path)
 img = tf.image.decode_jpeg(img, channels=3)
 return img
 def run_detector(detector, path):
 img = load_img(path)
 converted_img = tf.image.convert_image_dtype(img, tf.float32)[tf.newaxis, ...]
 start_time = time.time()
 result = detector(converted_img)
 end_time = time.time()
 result = {key:value.numpy() for key,value in result.items()}
 print("Found %d objects." % len(result["detection_scores"]))
 print("Processing time: ", end_time-start_time)
 image_with_boxes = draw_boxes(
 img.numpy(), result["detection_boxes"],
 result["detection_class_entities"], result["detection_scores"])
 display_image(image_with_boxes)
 if __name__ == "__main__":
 run_detector(detector, downloaded_image_path)
