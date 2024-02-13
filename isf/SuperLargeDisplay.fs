/*{
  "DESCRIPTION": "dreamseqs sphere mapper to show super large display",
  "CREDIT": "Kanon Kakuno",
  "CATEGORIES": [
    "dreamseqs"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 100.0
    }
  ]
  }*/

const float PI = 3.141592;

// @see https://github.com/dmnsgn/glsl-rotate/blob/main/rotation-3d.glsl
// @see https://wgld.org/d/glsl/g017.html
vec3 rotate(vec3 position, float angleRadian, vec3 axis) {
  axis = normalize(axis);
  float s = sin(angleRadian);
  float c = cos(angleRadian);
  float oc = 1.0 - c;

  mat3 rotateMatrix = mat3(
    oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
    oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
    oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c
  );

  return rotateMatrix * position;
}

vec3 rotateX(vec3 position, float angle) {
  return rotate(position, angle, vec3(1.0, 0.0, 0.0));
}

vec3 rotateY(vec3 position, float angle) {
  return rotate(position, angle, vec3(0.0, 1.0, 0.0));
}

vec3 rotateZ(vec3 position, float angle) {
  return rotate(position, angle, vec3(0.0, 0.0, 1.0));
}

vec3 rotateXYZ(vec3 position, vec3 angles) {
  return rotateZ(rotateY(rotateX(position, angles.x), angles.y), angles.z);
}

vec3 rotateXYZInvert(vec3 position, vec3 angles) {
  return rotateX(rotateY(rotateZ(position, -angles.z), -angles.y), -angles.x);
}

void main() {
  vec2 fragCoord = (vv_FragNormCoord.xy - vec2(0.5, 0.5)) * 2.0;

  vec3 ray = normalize(vec3(
    cos(PI * (-fragCoord.x + 0.5)) * cos(PI * fragCoord.y * 0.5),
    sin(PI * (-fragCoord.x + 0.5)) * cos(PI * fragCoord.y * 0.5),
    sin(PI * fragCoord.y * 0.5)
  ));

  ray = rotateX(ray, TIME);

  vec3 screenCenterPosition = vec3(0.0, 5.0, 0.0);
  vec3 screenRotation = vec3(0.0 * PI, 0.0, 0.0);
  vec3 screenNormal = rotateXYZ(vec3(0.0, 1.0, 0.0), screenRotation);

  float tParameter = -1 * dot(-screenCenterPosition, screenNormal) / dot(ray, screenNormal);
  vec3 screenFragPosition = ray * tParameter;
  vec3 texturePosition = rotateXYZInvert(screenFragPosition - screenCenterPosition, screenRotation);
  vec3 textureNormPosition = texturePosition / (vec3(3.2, 1.0, 1.8) * scale) + vec3(0.5, 0.0, 0.5);


  gl_FragColor = step(0, tParameter) * step(abs(texturePosition.x), 3.2 * scale) * step(abs(texturePosition.z), 1.8 * scale) * IMG_NORM_PIXEL(inputImage, textureNormPosition.xz);
}
