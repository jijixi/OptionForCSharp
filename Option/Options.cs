using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectUtility
{
    public static class Options
    {
        public static Option<T> make<T>(Func<T> f)
        {
            try {
                T v = f();
                if (v == null) {
                    return new None<T>();
                } else {
                    return new Some<T>(v);
                }
            } catch (Exception) {
                return new None<T>();
            }
        }

        public class Option<T> : IEnumerable<T>
        {
            public virtual bool IsAvailable
            {
                get { return false; }
            }

            public virtual T value
            {
                get { return default(T); }
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class None<T> : Option<T>
        {
        }

        public class Some<T> : Option<T>
        {
            public Some(T v)
            {
                this.value_ = v;
            }

            override public bool IsAvailable
            {
                get { return true; }
            }

            private T value_;
            override public T value
            {
                get
                {
                    return this.value_;
                }
            }
        }
    }
}
