import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from "@angular/common/http";

import { routing } from './app.routes';
import { AppService } from './app.service';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { CatalogModule } from './catalog/catalog.module';
import { OrdersModule } from './orders/orders.module';
import { BasketModule } from './basket/basket.module';
import { CampaignsModule } from './campaigns/campaigns.module';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserAnimationsModule,
        BrowserModule,
        ToastrModule.forRoot(),
        routing,
        HttpClientModule,
        // Only module that app module loads
        SharedModule.forRoot(),
        CatalogModule,
        OrdersModule,
        BasketModule,
        CampaignsModule
    ],
    providers: [
        AppService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
