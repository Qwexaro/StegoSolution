/// Copyright (c) 2026 Kveks (Qwexaro)

/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:

/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.

/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.


namespace Stego.Domain

open System

type CarrierKind =
    | Png
    | Bmp
    | WavPcm

type StegoError =
    | CarrierTooSmall of capacity: int64 * required: int64
    | InvalidMagicSignature of byte[]
    | UnsupportedCarrierFormat of string
    | AuthenticationFailed
    | CorruptedPayload of string
    | IoError of string

[<Flags>]
type PacketFlags =
    | None = 0uy
    | EncryptedAesGcm = 1uy // Bit 0
    | Compressed = 2uy      // Bit 1

type PacketHeader = {
    
    Magic: uint16          // 0x5354 ("ST")
    
    Version: byte          // 0x01
    
    Flags: PacketFlags
    
    PayloadLength: uint32  // Little-Endian

}

type CarrierSamples = {

    Kind: CarrierKind

    ModifiableBytes: byte[]
    
    RebuildCarrier: byte[] -> Result<byte[], StegoError>

}

type ICarrierCodec =
    abstract member CanHandle : fileExtension: string -> bool
    abstract member Decode : rawBytes: byte[] -> Result<CarrierSamples, StegoError>

type ICryptoService =
    abstract member Encrypt : plaintext: byte[] -> password: string -> aad: byte[] -> Result<byte[] * byte[], StegoError>
    abstract member Decrypt : ciphertext: byte[] -> password: string -> aad: byte[] -> authTag: byte[] -> Result<byte[], StegoError>

type StegoService = {

    Embed: carrier: byte[] -> ext: string -> payload: byte[] -> password: string option -> seed: int option -> Result<byte[], StegoError>
    
    Extract: carrier: byte[] -> ext: string -> password: string option -> seed: int option -> Result<byte[], StegoError>

}
