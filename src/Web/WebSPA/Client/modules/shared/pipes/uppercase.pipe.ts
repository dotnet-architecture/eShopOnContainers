import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
        name: 'appfUppercase'
})
export class UppercasePipe implements PipeTransform {
    transform(value: string) {
        return value.toUpperCase();
    }
}
