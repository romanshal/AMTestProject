import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { MeteoriteFiltersComponent, YEAR_ONLY_FORMATS } from './common/components/meteorite-filters/meteorite-filters.component';
import { MeteoriteTableComponent } from './common/components/meteorite-table/meteorite-table.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ResponseErrorInterceptor } from './common/interceptors/response-error-interceptor';
import { InternalServerErrorComponent } from './errors/internal-server-error/internal-server-error.component';
import { CatalogComponent } from './catalog/catalog.component';
import { MaterialModule } from './materials/material.module';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { provideNativeDateAdapter } from '@angular/material/core';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    MeteoriteFiltersComponent,
    MeteoriteTableComponent,
    InternalServerErrorComponent,
    CatalogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ResponseErrorInterceptor,
      multi: true
    },
    provideNativeDateAdapter(),
    { provide: MAT_DATE_FORMATS, useValue: YEAR_ONLY_FORMATS }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
