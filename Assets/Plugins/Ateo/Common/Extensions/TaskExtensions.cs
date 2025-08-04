using System;
using System.Threading;
using UnityEngine;

namespace Ateo.Extensions
{
    /// <summary>
    /// Provides helper methods for managing <see cref="CancellationToken"/> and <see cref="CancellationTokenSource"/> instances.
    /// </summary>
    public static class TaskHelper
    {
        #region Public Methods

        /// <summary>
        /// Refreshes the cancellation token by disposing of the current token source and creating a new one.
        /// </summary>
        /// <param name="tokenSource">The token source to refresh.</param>
        /// <returns>A new <see cref="CancellationToken"/>.</returns>
        public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource)
        {
            Kill(ref tokenSource);
            tokenSource = new CancellationTokenSource();
            return tokenSource.Token;
        }

        /// <summary>
        /// Refreshes the cancellation token by disposing of the current token source and creating a new one, with a log message on token registration.
        /// </summary>
        /// <param name="tokenSource">The token source to refresh.</param>
        /// <param name="message">The message to log when the token is registered.</param>
        /// <returns>A new <see cref="CancellationToken"/>.</returns>
        public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource, string message)
        {
            CancellationToken token = RefreshToken(ref tokenSource);

            token.Register(() =>
            {
                Debug.Log(message);
            });

            return token;
        }

        /// <summary>
        /// Refreshes the cancellation token and links it with a parent token.
        /// </summary>
        /// <param name="tokenSource">The token source to refresh.</param>
        /// <param name="token">The other cancellation token to link with.</param>
        /// <returns>A new linked <see cref="CancellationToken"/>.</returns>
        public static CancellationToken RefreshTokenAndLink(ref CancellationTokenSource tokenSource, CancellationToken token)
        {
            Kill(ref tokenSource);
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            return tokenSource.Token;
        }
        
        /// <summary>
        /// Refreshes the cancellation token and links it with a parent token.
        /// </summary>
        /// <param name="tokenSource">The token source to refresh.</param>
        /// <param name="tokens">The other cancellation token to link with.</param>
        /// <returns>A new linked <see cref="CancellationToken"/>.</returns>
        public static CancellationToken RefreshTokenAndLink(ref CancellationTokenSource tokenSource, params CancellationToken[] tokens)
        {
	        Kill(ref tokenSource);
	        tokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokens);
	        return tokenSource.Token;
        }

        /// <summary>
        /// Refreshes the cancellation token and links it with a parent token, with a log message on token registration.
        /// </summary>
        /// <param name="tokenSource">The token source to refresh.</param>
        /// <param name="token">The other cancellation token to link with.</param>
        /// <param name="message">The message to log when the token is registered.</param>
        /// <returns>A new linked <see cref="CancellationToken"/>.</returns>
        public static CancellationToken RefreshTokenAndLink(ref CancellationTokenSource tokenSource, CancellationToken token, string message)
        {
            CancellationToken cancellationToken = RefreshTokenAndLink(ref tokenSource, token);

            cancellationToken.Register(() =>
            {
                Debug.Log(message);
            });

            return cancellationToken;
        }

        /// <summary>
        /// Cancels and disposes of the provided token source if it has not already been canceled.
        /// </summary>
        /// <param name="tokenSource">The token source to kill.</param>
        public static void Kill(ref CancellationTokenSource tokenSource)
        {
            if (tokenSource != null && !tokenSource.IsCancellationRequested)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        /// <summary>
        /// Creates a linked cancellation token with a specified timeout.
        /// </summary>
        /// <param name="originalToken">The original cancellation token to link with.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds for the linked token.</param>
        /// <param name="onTimeout">Optional action to execute when the timeout occurs.</param>
        /// <returns>A tuple containing a new <see cref="CancellationToken"/> that is linked with the original token and the timeout, and a <see cref="CancellationTokenRegistration"/> for the callback.</returns>
        public static (CancellationToken linkedToken, CancellationTokenRegistration registration) CreateLinkedTokenWithTimeout(
            CancellationToken originalToken,
            float timeoutInSeconds,
            Action onTimeout = null)
        {
            CancellationTokenSource timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds));
            CancellationToken timedToken = timeoutSource.Token;

            CancellationTokenRegistration registration = default;
            if (onTimeout != null)
            {
                registration = timedToken.Register(onTimeout);
            }

            CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(originalToken, timedToken);
            return (linkedSource.Token, registration);
        }

        #endregion
    }
}
