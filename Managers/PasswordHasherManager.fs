namespace TodoApi.Managers

open BCrypt.Net

type PasswordHasher () =

    interface IPasswordHasherManager with

        /// <summary>Cipher a plain text</summary>
        /// <param name="plainText">Plain text</param>
        /// <returns>Ciphertext</returns>
        member this.Hash (plainText : string) =
            let ciphertext = BCrypt.HashPassword plainText
            ciphertext

        /// <summary>Checks if the plain text and ciphertext match</summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="ciphertext">Ciphertext</param>
        /// <returns>If both text match returns true</returns>
        member this.Compare (plainText : string) (ciphertext : string) =
            let isValid = BCrypt.Verify(plainText, ciphertext)
            isValid
