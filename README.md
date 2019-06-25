Printer Recognizer
====

## Description

This is a sample program which interacts with [Custom Vision](https://azure.microsoft.com/en-us/services/cognitive-services/custom-vision-service/) and recognizes what the printer vendor from a photo printer in.

## Requirement

* .Net Core SDK 2.2

## Usage

```
Usage: PrinterRecognizer [options]

Options:
  -?|-h|--help  Show help information
  -u|--url      URL to an image to be predicted
  -l|--local    Path to an image to be predicted
```

## Example

```
$ PrinterRecognizer -l /sample/images/printer_image.jpg

[Recognition Result]
Canon    :  61%
Epson    :  35%
HP       :   1%
Brother  :   1%
```

## Licence

[MIT](https://github.com/tcnksm/tool/blob/master/LICENCE)