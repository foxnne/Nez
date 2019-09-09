using System.Collections.Generic;
using System.Globalization;
using Nez;
using Nez.UI;
using Microsoft.Xna.Framework;

#if DEBUG
namespace Nez
{
    public class DirectionInspector : Inspector
    {
        //UI.TextField _textFieldValue, _textFieldName;
        SelectBox<string> _selectBox;
        UI.TextField _textField;

        public override void Initialize(Table table, Skin skin, float leftCellWidth)
        {
            var value = GetValue<Direction>();
            var label = CreateNameLabel(table, skin, leftCellWidth);

            _selectBox = new SelectBox<string>(skin);
            //_selectBox.SetWidth(50);

            _textField = new UI.TextField(value.Value.ToString(CultureInfo.InvariantCulture), skin);
            _textField.SetTextFieldFilter(new DigitsOnlyFilter()).SetPreferredWidth(50);
            _textField.OnTextChanged += (field, str) =>
            {
                int input;
                if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out input))
                {
                    Direction newValue = new Direction(input);
                    SetValue(newValue);
                }
            };

            var directions = new List<string>()
            {
                "S", "SE", "E","NE", "N","NW","W","SW"
            };

            _selectBox.SetItems(directions);

            _selectBox.OnChanged += selectedItem => { SetValue ((Direction)directions.IndexOf(selectedItem)); };

            var hBox = new HorizontalGroup(5);
            hBox.AddElement(_selectBox);
            hBox.AddElement(_textField);

            table.Add(label);
            table.Add(hBox);
        }

        public override void Update()
        {
            var value = GetValue<Direction>();
            _selectBox.SetSelected(value.ToString());
            _textField.SetText(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
#endif