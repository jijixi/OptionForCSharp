using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectUtility
{
    public static class Options
    {
        public static Option<T> Make<T>(Func<T> f)
        {
            if (f == null) {
                return new None<T>();
            }

            try {
                T v = f();
                if (v == null) {
                    return new None<T>();
                } else {
                    return new Some<T>(v);
                }
            } catch (NullReferenceException) {
                return new None<T>();
            }
        }

        public static Option<TResult> Select<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.Bind(selector);
        }

        public static Option<TResult> Bind<TSource, TResult>(
            this Option<TSource> self,
            Func<TSource, TResult> func)
        {
            if (self.IsAvailable) {
                return Make(() => func(self.Value));
            } else {
                return new None<TResult>();
            }
        }

        public static void Bind_<T>(
            this Option<T> source,
            Action<T> action)
        {
            if (source.IsAvailable) {
                action(source.Value);
            }
        }

        public abstract class Option<T> : IEnumerable<T>
        {
            public virtual bool IsAvailable
            {
                get { return false; }
            }

            public virtual T Value
            {
                get { return default(T); }
                protected set { /* ignore */ }
            }

            public virtual IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public sealed class None<T> : Option<T>
        {
            private class Enumerator<U> : IEnumerator<U>
            {
                public U Current
                {
                    get { return default(U); }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { throw new NotImplementedException(); }
                }

                public bool MoveNext()
                {
                    return false;
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }

                public void Dispose()
                {
                    return; // do nothing.
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                return new Enumerator<T>();
            }
        }

        public sealed class Some<T> : Option<T>
        {
            public Some(T v)
            {
                if (v == null) {
                    throw new InvalidOperationException();
                }
                this.Value = v;
            }

            public override bool IsAvailable
            {
                get { return true; }
            }

            public override T Value { get; protected set; }

            private class Enumerator<U> : IEnumerator<U>
            {
                private U value;
                private bool isReady = false;

                public Enumerator(U v)
                {
                    if (v == null) {
                        throw new InvalidOperationException();
                    }
                    this.value = v;
                }

                public U Current
                {
                    get { return isReady ? this.value : default(U); }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { throw new NotImplementedException(); }
                }

                public bool MoveNext()
                {
                    if (!isReady) {
                        isReady = true;
                        return true;
                    } else {
                        return false;
                    }
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }

                public void Dispose()
                {
                    return; // do nothing.
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                return new Enumerator<T>(this.Value);
            }
        }
    }
}
