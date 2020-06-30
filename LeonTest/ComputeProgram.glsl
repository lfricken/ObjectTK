#version 440
// type



#if TypeIs_VertexShader
uniform mat4 writeTransform;
uniform vec2 readTransform;

in vec4 position;
in vec2 texCoord;

out vec2 fragTexCoord;

void main(void)
{
    fragTexCoord = readTransform * texCoord;
    gl_Position = writeTransform * position;
}
#endif



#if TypeIs_FragmentShader
uniform sampler2D texture0;
uniform sampler2D texture1;

in vec2 fragTexCoord;

out vec4 outputColor;

void main(void)
{
    outputColor = mix(texture(texture1, fragTexCoord), texture(texture0, fragTexCoord), 0.8);
}
#endif
