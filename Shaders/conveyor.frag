#version 330 core
in vec2 TexCoord;

out vec4 FragColor;

uniform sampler2D textureSampler;
uniform vec4 objectColor;

void main()
{
    FragColor = texture(textureSampler, TexCoord) * objectColor;
}