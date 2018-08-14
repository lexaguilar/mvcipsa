var NamesOFMonth = {
    1: 'Enero',
    2: 'Febrero',
    3: 'Marzo',
    4: 'Abril',
    5: 'Mayo',
    6: 'Junio',
    7: 'Julio',
    8: 'Agosto',
    9: 'Septiembre',
    10: 'Octubre',
    11: 'Noviembre',
    12: 'Diciembre'
};

var customDate = function(_date)
{
    return {        
        NameOfMonth: function () { 
            var d = moment(_date).format('MM');
            return NamesOFMonth[parseInt(d)];
        }.apply(),
        Year: function () {
            var d = moment(_date).format('YYYY');
            return d;
        }.apply()
    }    
}