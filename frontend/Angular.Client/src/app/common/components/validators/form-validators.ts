import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function minYearValidator(minYear: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value instanceof Date) {
      const year = value.getFullYear();
      return year < minYear ? { minYear: { min: minYear, actual: year } } : null;
    }
    return null;
  };
}

export function maxYearValidator(maxYear: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value instanceof Date) {
      const year = value.getFullYear();
      return year > maxYear ? { maxYear: { max: maxYear, actual: year } } : null;
    }
    return null;
  };
}