import { NgModule, ModuleWithProviders }                     from '@angular/core';
import { BrowserModule  }               from '@angular/platform-browser';

import { SharedModule }                 from '../shared/shared.module';
import { BasketComponent }              from './basket.component';
import { BasketStatusComponent }        from './basket-status/basket-status.component';
//import { routing }                      from './basket.routes';
import { BasketService }                from './basket.service';

@NgModule({
    imports: [SharedModule],
    declarations: [BasketComponent, BasketStatusComponent], 
    providers: [BasketService],
    exports: [BasketStatusComponent]
})
export class BasketModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: BasketModule,
            providers: [
                // Providers
                BasketService
            ]
        };
    }
}
