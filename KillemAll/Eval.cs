using System;
using System.Threading;
using System.Threading.Tasks;

namespace KillemAll
{
    public static partial class Eval
    {

        public static TResult Invoke<TResult>(Func<TResult> f)
        {
            return f();
        }

        public static TResult Apply<T, TResult>(this T value, Func<T, TResult> f)
        {
            return f(value);
        }

        public static Result<T> TryInvoke<T>(Func<T> f)
        {
            return TryInvoke(f, default(T));
        }


        public static Result<T> TryInvoke<T>(Func<T> f, T @default)
        {
            try
            {
                return new Result<T> { Value = f(), Success = true };
            }
            catch (Exception ex)
            {
                return new Result<T> { Value = @default, Success = false, Exception = ex };
            }
        }


        public static Result<T> TryInvokeOrHandleException<T>(Func<T> f, Func<Exception, T> handler)
        {
            try
            {
                return new Result<T> { Value = f(), Success = true };
            }
            catch (Exception ex)
            {
                return new Result<T> { Value = handler(ex), Success = false, Exception = ex };
            }
        }


        public static Result TryInvoke(Action op)
        {
            try
            {
                op();
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Exception = ex };
            }
        }

        public static async Task<Result> TryInvokeAsync(Func<Task> f)
        {
            try
            {
                await f();
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Exception = ex };
            }
        }

        public static async Task<Result<T>> TryInvokeAsync<T>(Func<Task<T>> f)
        {
            try
            {
                return new Result<T> { Success = true, Value = await f() };
            }
            catch (Exception ex)
            {
                return new Result<T> { Success = false, Exception = ex };
            }
        }


        public static TResult Retry<TResult>(Func<TResult> f, int maxRetries, int retryInterval)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return f();
                }
                catch
                {
                    if (i < maxRetries - 1)
                        Thread.Sleep(retryInterval);
                }
            }

            throw new Exception(string.Format("Execution of {0} failed after trying {1} times", f, maxRetries));
        }


        public static void Retry(Action op, int maxRetries, int retryInterval)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    op();
                    return;
                }
                catch
                {
                    if (i < maxRetries - 1)
                        Thread.Sleep(retryInterval);
                }
            }

            throw new Exception(string.Format("Execution of {0} failed after trying {1} times", op, maxRetries));
        }

        public static async Task<TResult> RetryAsync<TResult>(Func<TResult> f, int maxRetries, int retryInterval)
        {
            return await Task.Run(() => Retry(f, maxRetries, retryInterval));
        }

        public static async Task RetryAsync(Action op, int maxRetries, int retryInterval)
        {
            await Task.Run(() => Retry(op, maxRetries, retryInterval));
        }


        public struct Result
        {
            public bool Success { get; set; }
            public Exception Exception { get; internal set; }
        }

        public struct Result<T>
        {
            public bool Success { get; set; }
            public Exception Exception { get; internal set; }
            public T Value { get; internal set; }

            public static implicit operator T(Result<T> @this)
            {
                return @this.Value;
            }

            public override string ToString()
            {
                return Convert.ToString(Value);
            }
        }
    }
}
