#version 330 core
in vec2 texCoord;

out vec4 outputColor;

uniform sampler2D text;
uniform vec4 textColor;

void main()
{
    float alpha = texture(text, texCoord).r;
    outputColor = vec4(textColor.rgb, textColor.a * alpha);
}
