# USCIValidator
法人和其他组织统一社会信用代码校验器，符合 GB 32100-2015、GB 11714-1997 规范。

## 使用方法
```
var code = "913706XXXXXXXXXXXX";
var ret = USCIValidator.Valid(code);
Console.WriteLine($"{code} is {(ret? "valid":"invalid")}.");
```

## 注意
仅对行政区划代码的前两位进行了有效性校验。
