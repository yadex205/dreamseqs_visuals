"use strict";

class Visual {
  #gl;
  #currentTimeMsUniformPosition;
  #intervalHandle = 0;
  #animationFrameHandle = 0;

  constructor() {
    const canvasEl = document.getElementById("canvas");
    const gl = canvasEl.getContext('webgl2');

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

    const currentTimeMsUniformPosition = gl.getUniformLocation(program, "currentTimeMs");
    this.#currentTimeMsUniformPosition = currentTimeMsUniformPosition;

    this.#draw();
  }

  run = () => {
    const fps = 30;
    const interval = 1000 / 30;

    window.clearInterval(this.#intervalHandle);

    this.#intervalHandle = window.setInterval(() => {
      window.cancelAnimationFrame(this.#animationFrameHandle);
      this.#animationFrameHandle = window.requestAnimationFrame(this.#draw);
    }, interval);
  }

  #createShader = (glShaderType, shaderSourceCode) => {
    const gl = this.#gl;

    console.debug(`Compile ${glShaderType === gl.VERTEX_SHADER ? "vertex" : "fragment"} shader`)

    const shader = gl.createShader(glShaderType);
    gl.shaderSource(shader, shaderSourceCode);
    gl.compileShader(shader);

    if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
      console.error(gl.getShaderInfoLog(shader));
    }

    return shader;
  }

  #draw = () => {
    const gl = this.#gl;
    const currentTimeMsUniformPosition = this.#currentTimeMsUniformPosition;
    const currentTimeMs = Date.now();

    gl.uniform1ui(currentTimeMsUniformPosition, currentTimeMs);
    gl.drawArrays(gl.TRIANGLE_FAN, 0, 4);
    gl.flush();
  }
}
