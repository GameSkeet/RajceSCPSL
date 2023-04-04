using RajceUI.Binder;
using RajceUI.Elements;
using RajceUI.Utils;

using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace RajceUI.UI
{
    public static partial class UIElements
    {
        public static DropdownElement Dropdown(Expression<Func<int>> targetExpression, IEnumerable<string> options)
            => Dropdown(ExpressionUtility.CreateLabelString(targetExpression), targetExpression, options);

        public static DropdownElement Dropdown(LabelElement label, Expression<Func<int>> targetExpression, IEnumerable<string> options)
            => Dropdown(label, UIInternalUtility.CreateBinder(targetExpression), options);


        public static DropdownElement Dropdown(Expression<Func<int>> targetExpression, Action<int> writeValue, IEnumerable<string> options)
            => Dropdown(ExpressionUtility.CreateLabelString(targetExpression), targetExpression.Compile(), writeValue, options);

        public static DropdownElement Dropdown(LabelElement label, Func<int> readValue, Action<int> writeValue, IEnumerable<string> options)
            => Dropdown(label, Binder.Binder.Create(readValue, writeValue), options);


        public static DropdownElement DropdownReadOnly(Expression<Func<int>> targetExpression, IEnumerable<string> options)
            => Dropdown(ExpressionUtility.CreateLabelString(targetExpression), UIInternalUtility.CreateReadOnlyBinder(targetExpression),
                options);

        public static DropdownElement DropdownReadOnly(LabelElement label, Func<int> readValue, IEnumerable<string> options)
            => Dropdown(label, Binder.Binder.Create(readValue, null), options);


        public static DropdownElement Dropdown(LabelElement label, IBinder<int> binder, IEnumerable<string> options)
        {
            var element = new DropdownElement(label, binder, options);
            UIInternalUtility.SetInteractableWithBinder(element, binder);
            return element;
        }
    }
}
