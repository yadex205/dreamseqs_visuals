/*{
  "DESCRIPTION": "dreamseqs sphere visual generator",
  "CREDIT": "Kanon Kakuno",
  "CATEGORIES": [
  "dreamseqs",
  "GENERATOR"
  ],
  "INPUTS": [
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

const vec3 lightDirection01 = normalize(vec3(-1.0, -1.0, 0.0));
const vec3 lightDirection02 = normalize(vec3(1.0, 1.0, 0.0));

float distanceFunc(vec3 position) {
  // sphere
  // return length(mod(position, 4.0) - 2.0) - 1.0;

  // cube
  // vec3 q = abs(mod(position, 8.0) - 4.0);
  // return length(max(q - vec3(0.5, 0.5, 2.0), 0.0)) - 0.1;

  // grid
  // vec3 q = abs(mod(position, 8.0) - 4.0);
  // return min(min(length(max(q - vec3(0.5, 0.5, 10.0), 0.0)) - 0.1, length(max(q - vec3(10.0, 0.5, 0.5), 0.0)) - 0.1), length(max(q - vec3(0.5, 10.0, 0.5), 0.0)) - 0.1);

  // grid
  vec3 q = abs(mod(position, 8.0) - 4.0);
  return min(length(max(q - vec3(0.5, 0.5, 10.0), 0.0)) - 0.1, length(max(q - vec3(10.0, 0.5, 0.5), 0.0)) - 0.1);
}

vec3 normalFunc(vec3 position) {
  float delta = 0.001;
  return normalize(vec3(
    distanceFunc(position + vec3(delta, 0.0, 0.0)) - distanceFunc(position - vec3(delta, 0.0, 0.0)),
    distanceFunc(position + vec3(0.0, delta, 0.0)) - distanceFunc(position - vec3(0.0, delta, 0.0)),
    distanceFunc(position + vec3(0.0, 0.0, delta)) - distanceFunc(position - vec3(0.0, 0.0, delta))
  ));
}

void main() {
  vec2 fragmentPosition = (vv_FragNormCoord.xy - vec2(0.5, 0.5)) * 2.0;

  float progress = mod(TIME, 16.0) / 16.0;

  vec3 cameraPosition = vec3(0.0, -3.0 + 32.0 * progress, 0.0);

  vec3 ray = normalize(vec3(
    cos(PI * (-fragmentPosition.x + 0.5)) * cos(PI * fragmentPosition.y * 0.5),
    sin(PI * (-fragmentPosition.x + 0.5)) * cos(PI * fragmentPosition.y * 0.5),
    sin(PI * fragmentPosition.y * 0.5)
  ));

  ray = rotateY(ray, sin(progress * PI * 2.0));

  float distance = 0.0;
  float rayLength = 0.0;
  vec3 rayPosition = cameraPosition;

  for (float i = 1.0; i < 32.0; i++) {
    distance = distanceFunc(rayPosition);
    rayLength = rayLength + distance;
    rayPosition = cameraPosition + ray * rayLength;
  }

  vec3 normalDirection = normalFunc(rayPosition);
  float brightness = min((50.0 - rayLength) / 50.0, 5.0) * step(rayLength, 50.0) * step(distance, 0.001) * clamp(dot(lightDirection01, normalDirection), 0.1, 1.0);
  brightness = brightness + min((50.0 - rayLength) / 50.0, 5.0) * step(rayLength, 50.0) * step(distance, 0.001) * clamp(dot(lightDirection02, normalDirection), 0.1, 1.0);

  gl_FragColor = vec4(
    brightness * 1.0,
    0.0, // brightness,
    0.0, // brightness,
    1.0
  );
}
