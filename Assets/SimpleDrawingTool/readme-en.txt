/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_
// Simple Drawing Tool
// © 2019 Kazuya Hiruma
// Version 1.0.0
/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_

========================================================================================
## Update history

Version 1.0.0:
- First release.
========================================================================================


Simple Drawing Tool provide a simple way to draw free-hand on uGUI.


## Getting started

You can test how to use this asset easily, please put a "SimpleDrawingTool" prefab into a scene that you want.
The prefab include all of samples what this asset can do.


## How to use by script

This asset can be controled by script. The features are below.

1. Change a brush
2. Change a color
3. Change size
4. Clear canvas
5. Undo drawing
6. Save the canvas as a PNG file.


### Interfaces for controling the tool

You can control above features via some interfaces like below.


#### Syntax

- void SimpleDrawingTool.SetBrush(Texture2D brush);

This method will set a texture as brush.


- void SimpleDrawingTool.SetBrushSize(float size);

This method will set size for drawing brush.


- void SimpleDrawingTool.SetColor(Color color);

This method will set a color for drawing.


- void SimpleDrawingTool.Undo();

This method will pop the last drawing.


- void SimpleDrawingTool.Clear();

This method will clear the canvas.


If you want to know how to do scripting, please check it out on "DrawController" script.