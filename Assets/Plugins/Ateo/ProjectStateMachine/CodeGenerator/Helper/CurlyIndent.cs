#if UNITY_EDITOR
using System;
using System.Text;

namespace Ateo.CodeGeneration
{
    public class CurlyIndent : IDisposable 
    {
        public WrappedInt Val;
        private readonly StringBuilder _builder;

        public CurlyIndent(StringBuilder b, WrappedInt counter)
        {
            Val = counter;
            _builder = b;
            _builder.AppendIndentLine(Val, "{");
            ++Val;
        }

        public void Dispose()
        {
            --Val;
            _builder.AppendIndentLine(Val, "}");
        }
    }

    public class WrappedInt
    {
        private int _value;

        public WrappedInt Store(int num)
        {
            _value = num;
            return this;
        }

        public int Load()
        {
            return _value;
        }

        public static implicit operator WrappedInt(int val)
        {
            return new WrappedInt().Store(val);
        }

        public static implicit operator int(WrappedInt val)
        {
            return val.Load();
        }

        public static WrappedInt operator ++(WrappedInt self)
        {
            self._value++;
            return self;
        }

        public static WrappedInt operator --(WrappedInt self)
        {
            self._value--;
            return self;
        }

    }
}
#endif
