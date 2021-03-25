MeetBarracuda
=============

**MeetBarracuda** is a human segmentation filter that runs the
[MediaPipe Meet Segmentation Model] on the [Unity Barracuda] neural network
inference library.

[MediaPipe Meet Segmentation Model]:
  https://drive.google.com/file/d/1lnP1bRi9CSqQQXUHa13159vLELYDgDu0/view

[Unity Barracuda]:
  https://docs.unity3d.com/Packages/com.unity.barracuda@latest

About the ONNX model
--------------------

The segmentation model was converted into ONNX by PINTO0309 (Katsuya Hyodo).
Please check [his model zoo] for further details.

[his model zoo]: https://github.com/PINTO0309/PINTO_model_zoo

About the license
-----------------

Even though the repository contains the original license (Apache 2.0), the
original developer (Google) changed it to [Google Terms of Service] after the
initial release.

[Google Terms of Service]: https://policies.google.com/terms?hl=en-US

There are some threads trying to make it clear if we can use the model in
commercial/non-commercial projects, but at the moment, there is no clear
conclusion.

[some threads]: https://github.com/PINTO0309/PINTO_model_zoo/issues/69

I wouldn't recommend using it in a practical application.
