using System;
using System.Collections.Generic;

namespace RPNCalulator {
	public class RPN {
		private Stack<int> _operators; 
		Dictionary<string, Func<int, int, int>> _operationFunctionTwoNumber; //  _operationFunction jako słownik dla 2
		Dictionary<string, Func<int, int>> _operationFunctionSingleNumber; //  _operationFunction jako słownik dla 1
		static Func<int, int> factorial = n => n < 2 ? 1 : n * factorial(n - 1);

		public int EvalRPN(string input) { // Funkcja wykonująca dla obu opcji jedno i dwu arg
			_operationFunctionSingleNumber = new Dictionary<string, Func<int, int>> // dla int i znak -obiekt typu słownik
			{
				["!"] = (fst) => (fst < 0 ? throw new InvalidOperationException() : factorial(fst)),
				["||"] = (fst) => (Math.Abs(fst))
			};

			_operationFunctionTwoNumber = new Dictionary<string, Func<int, int, int>> // nowy słownik dla dwóch
			{
				["+"] = (fst, snd) => (fst + snd), // Jeżeli kluczem jest + to weź argument fst oraz snd i wykonaj fst + snd
				["-"] = (fst, snd) => (fst - snd), 
				["*"] = (fst, snd) => (fst * snd), 
				["/"] = (fst, snd) => (snd == 0 ? throw new DivideByZeroException() : fst < snd ? throw new InvalidOperationException() : fst / snd)
				//  dzielenie z wyjątkami  gdy dziel przez 0, gdy pierwsza jest mniejsza niz druga
			};
			_operators = new Stack<int>(); // nowy stos

			var splitInput = input.Split(' '); // dzieli input na pojedyncze znaki
			foreach (var op in splitInput)
			{
				if (IsNumber(op)) // Jeśli znak jest liczbą
					_operators.Push(Int32.Parse(op)); // wrzuć  na stos
				else
				if (IsOperatorForSingleNumber(op)) // Jeśli znak jest operatorem dla jednego argumentu np. silnia
				{
					var num = _operators.Pop(); // Zdejmij ze stosu 
					_operators.Push(_operationFunctionSingleNumber[op](num)); //  na stos wynik działania w _operationFunction wg odczyt. operatora
				}
				else
				if (IsOperatorForTwoNumbers(op)) // i operator dla dwóch arg i op
				{
					var num2 = _operators.Pop();
					var num1 = _operators.Pop(); 

					_operators.Push(_operationFunctionTwoNumber[op](num1, num2)); // Wrzuć na stos wynik ( dwóch intów i operator )
					
				}
			}

			var result = _operators.Pop(); // result = zdejmij ze stosu wynik
			if (_operators.IsEmpty)
			{
				return result;
			}
			throw new InvalidOperationException();
		}

		private bool IsNumber(String input) => Int32.TryParse(input, out _); // Funkcja sprawdzająca czy znak jest liczbą

		private bool IsOperatorForTwoNumbers(String input) => 
			input.Equals("+") || input.Equals("-") ||
			input.Equals("*") || input.Equals("/");

		private bool IsOperatorForSingleNumber(String input) => 
			input.Equals("!") || input.Equals("||");

		private Func<int, int, int> Operation(String input) =>
			(x, y) =>
			(
				(input.Equals("+") ? x + y :
					(input.Equals("*") ? x * y : int.MinValue)
				)
			);
	}
}
