"use strict";

class Visual {
  #gl;

  constructor() {
    const canvasEl = document.getElementById("canvas");
    const gl = canvasEl.getContext('webgl');

    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clearDepth(1.0);
    gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

    this.#gl = gl;
  }

  setShaderSourceCode = (vertexShaderSourceCode, fragmentShaderSourceCode) => {
    const gl = this.#gl;

    const program = gl.createProgram();
    const vertexShader = this.#createShader(gl.VERTEX_SHADER, vertexShaderSourceCode);
    const fragmentShader = this.#createShader(gl.FRAGMENT_SHADER, fragmentShaderSourceCode);

    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragmentShader);
    gl.linkProgram(program);
    gl.useProgram(program);

    const positionAttributeLocation = gl.getAttribLocation(program, "position");
    const vertexPositions = [
      -1.0, 1.0,
      1.0, 1.0,
      1.0, -1.0,
      -1.0, -1.0,
    ];
    const vbo = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, vbo);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexPositions), gl.STATIC_DRAW);
    gl.enableVertexAttribArray(positionAttributeLocation);
    gl.vertexAttribPointer(positionAttributeLocation, 2, gl.FLOAT, false, 0, 0);

    gl.drawArrays(gl.TRIANGLE_FAN, 0, 4);
    gl.flush();
  }

  #createShader = (glShaderType, shaderSourceCode) => {
    const gl = this.#gl;

    const shader = gl.createShader(glShaderType);
    gl.shaderSource(shader, shaderSourceCode);
    gl.compileShader(shader);

    return shader;
  }
}
