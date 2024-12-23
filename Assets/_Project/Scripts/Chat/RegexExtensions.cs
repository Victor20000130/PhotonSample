using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class RegexExtensions
{
	//Regex : Regular Expression (정규 표현식)
	//이거 안쓰면 구글 플레이스토어에 못올림
	//비속어 필터 써야해서..

	//닉네임으로 0~9, a~z, A~Z, 가~힣 안에 포함되는
	//완성형 한글과 영문/숫자만 포함되는 정규식
	static Regex nicknameRegex = new Regex(@"[^0-9A-Za-z가-힣]");

	//Regex 사용하는 방법
	//1.Regex 클래스 사용
	public static bool NicknameValidate(this string nickname)
	{   //위에 선언된 nicknameRegex에 포함되는 놈이 하나도 없으면
		//리턴 값을 true로 주는 것임.
		return false == nicknameRegex.IsMatch(nickname);
	}

	//2.Regex 포맷 문자열로 변환
	//문자열 입력중에 미완성형 한글을 허용하는 정규식
	//모바일 환경은 천지인 자판까지 고려해서 ㆍᆞᆢ를 넣어주어야 함.
	const string INPUT_FORM = @"[^0-9A-Za-z가-힣ㄱ-ㅎㅏ-ㅣㆍᆞᆢ]";

	public static string ToValidString(this string param)
	{
		return Regex.Replace(param, INPUT_FORM, "", RegexOptions.Singleline);
	}

	//위대한 한글을 천박하게 쓰는 녀석들을 처단
	const string COMPLETE_HANGUL = @"[^가-힣]";

	//일반 비속어
	static List<string> fword = new()
	{
		"씨발",
		"Tlqkf",
		"tlqkf",
		"시발"

	};

	//변형 비속어
	static List<string> irregularFword = new()
	{
		"ㅅH77l",
		"시1발",
		"씨1발"
	};

	public static bool ContainsFword(this string param)
	{
		if (string.IsNullOrEmpty(param)) return false;

		//변형 비속어 먼저 검사.
		if (irregularFword.Exists(x => param.Contains(x))) return true;

		//완성형 한글만 남김 예 : 예쁜1말 => 예쁜말
		param = Regex.Replace(param, COMPLETE_HANGUL, "", RegexOptions.Singleline);

		return fword.Exists(x => param.Contains(x));
	}
}
