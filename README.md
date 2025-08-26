# USCIValidator
法人和其他组织统一社会信用代码校验器，符合 GB 32100-2015、GB 11714-1997 规范。
可用于对营业执照等社会信用代码进行有效性校验，如 OCR 识别、输入校验等。

## 使用方法
```
var code = "913706XXXXXXXXXXXX";
var ret = USCIValidator.Valid(code);
Console.WriteLine($"{code} is {(ret? "valid":"invalid")}.");
```

## 关于行政区划代码的有效性校验
仅对“登记管理机关行政区划代码”的前两位进行了区划有效性校验，如需要更准确的区划有效性校验，可自行修改。
