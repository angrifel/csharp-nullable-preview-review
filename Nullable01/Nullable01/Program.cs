using System;

namespace Nullable01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        /*
         * below are a list of expectations based on my personal wishlist on how things should
         * change on the next version of c# to implement nullable references
         * 
         * some of this expectations may require changes to CLR.
         * 
         * the changes would be
         * - null checks should be the new default. projects migrating from older versions that
         *   face problems with the new feature should opt-out instead of opting in.
         * - when there is absolute certainty of a null check error the compiler should issue an error.
         *   if something could be null but the compiler cannot prove it then a warning should be issued.
         * - default(T) should call the parameterless constructor of T -or- initialize using a static member marked to that effect
         * - default(T) without the above should result in a compilation error
         * - default(T?) should always return null
         * - new T[length] where T is a struct should invoke parameterless constructor to initialize the array.
         *   - the compile should warn when initializing an array
         *     with a parameterless constructor that is not the default
         * - new T[length] where T is a class should be initialized with null and the compiler should
         *   perform flow analysis that ensures all elements are assigned non-nullable element before leaving a scope.
         *   - if the compiler cannot prove the array is fully initialized before leaving a scope.
         *     either because there is a flaw in the initialization logic or 
         *     because the initialization logic cannot be proven to be correct
         *     it should emit an error;
         * - consider allowing parameterless constructor for struct so as to have parity with classes 
         * 
         * below is some code that capture some of these intentions
         */
        
        public static string GetString() => "somestring"; // ok
        public static string? GetNullableString1() => null; // ok
        public static string? GetNullableString2() => GetString(); // ok
        public static string GetDefaultString() => default; // not ok - should not compile - no parameterless constructor present in System.String
        public static string? GetDefaultNullableString() => default; // ok
        public static string[] GetArrayOfString() => new string[] { "", "" }; // ok
        public static string?[] GetArrayOfNullableString1() => new string?[] { null, "" };
        public static string?[] GetArrayOfNullableString2() => GetArrayOfString(); // should not warn
        public static string[]? GetNullableArrayOfString1() => null; // ok
        public static string[]? GetNullableArrayOfString2() => GetArrayOfString(); // ok
        public static string?[]? GetNullableArrayOfNullableString1() => null; // ok
        public static string?[]? GetNullableArrayOfNullableString2() => GetArrayOfString(); // should not warn
        public static string?[]? GetNullableArrayOfNullableString3() => GetNullableArrayOfString1(); // should not warn
        public static string?[]? GetNullableArrayOfNullableString4() => GetNullableArrayOfString2(); // should not warn
        public static string[] GetArrayWithEmptyInitialization() => new string[10]; // not ok - should not compile
        public static string[] GetArrayWithSimpleInitialization() // ok
        {
            var result = new string[10];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = "";
            }

            return result;
        }

        public static string[] GetArrayWithIncompleteInitialization() // not ok - should not compile
        {
            var result = new string[10];
            for (int i = 1; i < result.Length; i++)
            {
                result[i] = "";
            }

            return result;
        }

        public static T GetT<T>() where T : new() => new T(); //ok
        //public static T? GetNullableT1<T>() where T : new() => null; // not ok - it does not compile but it should. null is guaranteed to have meaning for T? since it is nullable
        public static T? GetNullableT2<T>() where T : new() => GetT<T>(); // ok
        public static T GetDefaultT1<T>() where T : new() => default; // not ok - should invoke parameterless constructor
        public static T GetDefaultT2<T>() => default; // not ok - should not compile
        public static T? GetDefaultNullableT<T>() where T : new() => default; // not ok - default(T?) is nullable already
    }
}
