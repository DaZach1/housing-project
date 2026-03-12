import { Routes } from '@angular/router';
import { AllHousesComponent } from './components/all-houses/all-houses.component';
import { HouseDetailComponent } from './components/house-detail/house-detail.component';
import { ApartmentDetailComponent } from './components/apartment-detail/apartment-detail.component';
import { ResidentDetailComponent } from './components/resident-detail/resident-detail.component';
import { HouseCreateComponent } from './components/house-create/house-create.component';
import { ApartmentCreateComponent } from './components/apartment-create/apartment-create.component';
import { ResidentCreateComponent } from './components/resident-create/resident-create.component';
import { LoginComponent } from './components/login/login.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent, },
  { 
    path: 'all-houses', 
    component: AllHousesComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Resident'] }
  },
  { 
    path: 'house/:id', 
    component: HouseDetailComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Resident'] }
  },
  { 
    path: 'apartment/:id', 
    component: ApartmentDetailComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Resident'] }
  },
  { 
    path: 'resident/:id', 
    component: ResidentDetailComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Resident'] }
  },
  { 
    path: 'house_create', 
    component: HouseCreateComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager'] }
  },
  { 
    path: 'house/:houseId/apartment/create', 
    component: ApartmentCreateComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager'] }
  },
  { 
    path: 'resident_create', 
    component: ResidentCreateComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager'] }
  },
  { 
    path: 'apartment/:apartmentId/resident/create',
    component: ResidentCreateComponent,
    canActivate: [authGuard],
    data: { roles: ['Manager'] }
  },
  { path: '', redirectTo: 'all-houses', pathMatch: 'full' },
  { path: '**', redirectTo: 'all-houses' }
];