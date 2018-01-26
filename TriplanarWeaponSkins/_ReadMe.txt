READ ME-
*Made using Amplify Shader Editor*


The shader applies a texture on top of a base metal using triplanar mapping
Base scratches are determined by the luminosity of the _BaseScratches texture and the _BaseScratchesCutoff value

Additional scratches are applied in the same way using the _AdditionalScratches texture

The overlay texture's transparency will be shaded in a color, the _Tint

The shader does not include a texture to be used by the base metal for optimization purposes (and I didn't need that)