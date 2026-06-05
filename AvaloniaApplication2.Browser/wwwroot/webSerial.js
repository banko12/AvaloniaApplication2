let serialPort = null;
let keepReading = false;
let reader = null;

// Prompts browser device-picker UI and configures the connection
export async function openSerialPort(baudRate) {
    try {
        serialPort = await navigator.serial.requestPort();
        await serialPort.open({ baudRate: parseInt(baudRate) });
        return true;
    } catch (err) {
        console.error("Web Serial opening error:", err);
        return false;
    }
}

/**
 * Writes a byte buffer out to the selected COM port.
 * @param {ArrayLike<number> | Uint8Array | number[]} buffer Byte values 0..255
 */
export async function writeBuffer(buffer) {
    if (!serialPort || !serialPort.writable) return;

    const writer = serialPort.writable.getWriter();
    try {
        // Accept Uint8Array directly, otherwise convert common JS shapes (number[], ArrayLike)
        const bytes = buffer instanceof Uint8Array ? buffer : Uint8Array.from(buffer);
        await writer.write(bytes);
    } finally {
        writer.releaseLock();
    }
}


// Writes text data out to the selected COM port
export async function writeSerialData(textData) {
    if (!serialPort || !serialPort.writable) return;

    const writer = serialPort.writable.getWriter();
    const encoder = new TextEncoder();
    await writer.write(encoder.encode(textData));
    writer.releaseLock();
}

// Begins an asynchronous read loop, passing bytes back to C#
export async function startReadLoop() {
    if (!serialPort || !serialPort.readable) return;

    keepReading = true;
    reader = serialPort.readable.getReader();

    try {
        while (keepReading) {
            const { value, done } = await reader.read();
            if (done) break;

            if (value) {
                // Pass raw bytes to the C# global listener exported by Avalonia
                globalThis.DotNetSerialListener.receiveBytes(Array.from(value));
            }
        }
    } catch (error) {
        console.error("Web Serial reading error:", error);
    } finally {
        reader.releaseLock();
    }
}

// Safely terminates the streams and releases the hardware port
export async function closeSerialPort() {
    keepReading = false;
    if (reader) {
        await reader.cancel();
    }
    if (serialPort) {
        await serialPort.close();
    }
}
