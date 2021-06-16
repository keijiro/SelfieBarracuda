SelfieBarracuda
===============

![gif](https://user-images.githubusercontent.com/343936/122239428-e3451280-cefb-11eb-8f77-1633226477c3.gif)

**SelfieBarracuda** is a human segmentation filter that runs the
[MediaPipe Selfie Segmentation Model] on the [Unity Barracuda] neural network
inference library.

[MediaPipe Selfie Segmentation Model]:
  https://google.github.io/mediapipe/solutions/selfie_segmentation.html

[Unity Barracuda]:
  https://docs.unity3d.com/Packages/com.unity.barracuda@latest

About the ONNX file
-------------------

I converted the original model (provided in tflite format) into ONNX using
[tflite2tensorflow].

[tflite2tensorflow]: https://github.com/PINTO0309/tflite2tensorflow

This repository only contains the landscape model (256x144). There is
another project called [SelfieSegmentationBarracuda] that contains the general
model (256x256), which is better for square/portrait aspect ratios.

[SelfieSegmentationBarracuda]:
  https://github.com/creativeIKEP/SelfieSegmentationBarracuda
