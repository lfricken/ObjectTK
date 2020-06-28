#version 440
// type



#if VertexShader
uniform mat4 transform;

in vec4 position;
in float other;

void main(void)
{
    gl_Position = transform * position;
    gl_Position.x += 0.4 * other;
}
#endif



#if FragmentShader
uniform sampler2D texture0;

out vec4 outputColor;

void main(void)
{
    outputColor = vec4(1.0, 0.0, 0.0, 1.0);
}
#endif
