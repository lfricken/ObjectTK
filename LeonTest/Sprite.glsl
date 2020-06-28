#version 440
// type



#if TypeIs_VertexShader
uniform mat4 transform;

in vec4 position;
in float other;
in vec2 _texCoord;

out vec2 texCoord;

void main(void)
{
    texCoord = _texCoord;
    gl_Position = transform * position;
    gl_Position.x += 0.4 * other;
}
#endif



#if TypeIs_FragmentShader
uniform sampler2D texture0;

in vec2 texCoord;

out vec4 outputColor;

void main(void)
{
    outputColor = texture(texture0, texCoord);
}
#endif
