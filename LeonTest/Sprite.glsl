#version 440
// type



#if TypeIs_VertexShader
uniform mat4 writeTransform;
uniform vec2 readTransform;

in vec4 position;
in vec2 _texCoord;

out vec2 texCoord;

void main(void)
{
    texCoord = readTransform * _texCoord;
    gl_Position = writeTransform * position;
}
#endif



#if TypeIs_FragmentShader
uniform sampler2D texture0;
uniform sampler2D texture1;

in vec2 texCoord;

out vec4 outputColor;

void main(void)
{
    vec4 x = texture(texture0, texCoord);
    outputColor = texture(texture1, texCoord);
}
#endif
