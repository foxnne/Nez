Nez Atlas Packer
==========
Sprite Atlas Packer supports reading PNG, JPG, BMP, and GIF images and produces a single PNG/JPG/BMP image with all the images embedded inside of it. The tool also produces an accompanying file that maps the image file names with their rectangles and any subfolders mapped to animations.

Nez includes runtime classes to load the output of Sprite Atlas Packer. When using with the Nez runtime loader make sure the output image and atlas have the same name.

Sprite Atlas Packer also handles animations. Any subdirectories that contain images will be setup as animations when packing the sprites into an atlas. In the example folder structure below the tool will generate the animations "player", "enemy1" and "enemy2" with any images present in those folders.

- root-dir
	- player
	- enemy1
	- enemy2


## Usage

`mono SpriteAtlasPacker.exe -image:out.png -map:out.atlas -fps:7 folder/with/images`


## Options

```
<input>           Images to pack. Default:''
/image:string     Output file name for the image.
/map:string       Output file name for the map.
/mw:int           Maximum ouput width. Default:'4096'
/mh:int           Maximum ouput height. Default:'4096'
/pad:int          Padding between images. Default:'1'
/pow2             Ensures output dimensions are powers of two.
/sqr              Ensures output is square.
/originX:float    Origin X for the images Default:'0.5'
/originY:float    Origin Y for the images Default:'0.5'
/fps:int          Framerate for any animations Default:'8'
/lua              Output LOVE2D lua file
```



### Attribution
Forked from this old bit of code from Codeplex: https://archive.codeplex.com/?p=spritesheetpacker


Nez Palette Packer
==========
Palette Packer supports reading PNG, JPG, BMP, and GIF images and produces a single PNG/JPG/BMP image with all the palette images embedded inside of it. The tool also produces an accompanying file that maps the palette image file names with their row indices.

Nez includes a t4 template to generate code to easily access the row of the palette as a constant integer.

Palette Packer will traverse folder structures and process any found images to pack into the final palette. If no width is specified, the first encountered image width will be the final palette width. An error will be returned and the program will exit if subsequent images do not match the width, or the image heights are not 1 px tall.

- root-dir
	- palette1 	(4px wide and 1 px tall)
	- palette2	(4px wide and 1 px tall)
	- palette3	(4px wide and 1 px tall)


## Usage

`mono PalettePacker.exe -image:out.png -map:out.atlas -width:4 -height:256 -toppadding:1 folder/with/images`


## Options

```
<input>           Images to pack. Default:''
/image:string     Output file name for the image.
/map:string       Output file name for the map.
/width:int        Input/ouput width.
/height:int       Ouput height.
/toppadding:int   Padding at the top of the final image. Default:'1'
```
