#Область ОбработчикиСобытий

Процедура ПриЗагрузкеБиблиотеки(Путь, СтандартнаяОбработка, Отказ)
	
	СтандартнаяОбработка = Ложь;

	Если ЭтоВерсия20() Тогда
		Местоположение = ОбъединитьПути(Путь, "bin", "net5.0", "CoverageBSL.AddIn.dll");
	Иначе
		Местоположение = ОбъединитьПути(Путь, "bin", "net48", "CoverageBSL.AddIn.dll");
	КонецЕсли;
	
	ПодключитьВнешнююКомпоненту(Местоположение);
	
КонецПроцедуры

Функция ЭтоВерсия20() 

	СистемнаяИнформация = Новый СистемнаяИнформация();
	Возврат СтрНачинаетсяС(СистемнаяИнформация.Версия, "2.");

КонецФункции

#КонецОбласти
