Printer Recognizer
====

## Description

This is a sample program which interacts with [Custom Vision](https://azure.microsoft.com/en-us/services/cognitive-services/custom-vision-service/) and recognizes what the manufacturer(HP/Canon/Epson/Brother) of the printer in a photo.

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

### note
Two environment variables `ENDPOINT` and `PREDICTION_KEY` are defined in `.env`.<br>
To run this program, You need to set the prediction key for the custom vision project as `PREDICTION_KEY` (but it's secret...) and put the `.env` into same folder where `PrinterRecognizer` exists.

## Example

```
$ PrinterRecognizer -l ./assets/images/printer_image.jpg

[Recognition Result]
Canon    :  61%
Epson    :  35%
HP       :   1%
Brother  :   1%
```

## Licence

MIT