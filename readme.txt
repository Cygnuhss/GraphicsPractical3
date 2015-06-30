
-- Graphics Practical 3 --
	30-06-2015

Yoni Groosman 	- 4101421
Jelmer van Nuss - 4058925

-------------------------
------- Teamwork --------
-------------------------

All of the work was done together, either in real life or through internet
communication.

-------------------------
------ Assignments ------
-------------------------

Camera - Done
See Game1.cs -> HandleInput, NextView and DrawText
Problem: going to the next view does not seem to work properly.
Next views can also be set via Initialize and a restart of the program.

E.1 Multiple light sources - Done
See Simple.fx
Lights (with color) can be set in Game1.cs

E.3 Cel shading - Started
See Simple.fx -> SimplePixelShader
Sources:
http://rbwhitaker.wikidot.com/toon-shader

E.5 Simple color filter - Done
See PostProcessing.fx -> GrayscalePixelShader
Sources:
http://rbwhitaker.wikidot.com/post-processing-effects

E.6 Gaussian blur - Started
See PostProcessing.fx and GaussianBlur.cs
Problem: the effects needs to receive the render target as a texture,
so that it can use that texture for lookup in the process of blurring.
Sources:
http://blogs.mathworks.com/steve/2006/10/04/separable-convolution/
http://www.songho.ca/dsp/convolution/convolution2d_example.html
http://www.dhpoware.com/demos/xnaGaussianBlur.html#_blank
