using System;
using Modding.Humankind.DevTools;

namespace StyledGUI
{

    public interface IGrid<TDataType>
    {
        TDataType Data { get; set; }
    }
    
    public abstract class BaseClass<TDataType> : IGrid<TDataType>
    {

        public TDataType Data { get; set; }

        public BaseClass<TDataType> A()
        {
            return this;
        }

        public BaseClass<TDataType> B(Action<TDataType, int> action = null)
        {
            action?.Invoke((TDataType)Data, 32);
            
            return this;
        }
    }

    public class DerivedClass<TDataType> : BaseClass<TDataType>
    {
        public IGrid<TDataType> DataContainer { get; set; }

        public DerivedClass<TDataType> C()
        {
            return this;
        }

        public DerivedClass<TDataType> D() => this;
    }

    public static class GridEx
    {
        public static BaseClass<T> E<T>(this BaseClass<T> self)
        {
            Loggr.Log("IN E!!");

            return self;
        }
    }

    

    
}