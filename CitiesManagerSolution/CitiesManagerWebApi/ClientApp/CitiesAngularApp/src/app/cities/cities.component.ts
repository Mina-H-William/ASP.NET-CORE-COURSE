import { Component } from '@angular/core';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.css']
})
export class CitiesComponent {
  cities: City[] = [];
  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;

  putCityForm: FormGroup;
  editCityID: string | null = null;

  constructor(private citiesService: CitiesService, private accountService: AccountService) {
    this.postCityForm = new FormGroup(
      {
        cityName: new FormControl(null, [Validators.required]),
      });

      this.putCityForm = new FormGroup(
        {
          cities: new FormArray([])
        });
  }

  get putCityFormArray(): FormArray {
    return this.putCityForm.get("cities") as FormArray;
  }

  get postCity_CityNameControl(): any {
    return this.postCityForm.controls['cityName'];
  }


  loadCities() {
    this.citiesService.getCities().subscribe({
      next: (response: City[]) => {
        this.cities = response;

        this.cities.forEach(city => {
          this.putCityFormArray.push(new FormGroup({
            cityID: new FormControl(city.cityID, [Validators.required]),
            cityName: new FormControl({ value: city.cityName, disabled: true }, [Validators.required])
          }));
        })
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }

  ngOnInit() {
    this.loadCities();
  }

  public postCitySubmitted() {
    this.isPostCityFormSubmitted = true;

    console.log(this.postCityForm.value);

    this.citiesService.postCity(this.postCityForm.value).subscribe({
      next: (response: City) => {
        console.log(response);

        this.cities.push(new City(response.cityID, response.cityName));

        this.putCityFormArray.push(new FormGroup({
          cityID: new FormControl(response.cityID, [Validators.required]),
          cityName: new FormControl({ value: response.cityName, disabled: true }, [Validators.required])
        }));
      },

      error: (error: any) => { console.log(error); },

      complete: () => {
        this.isPostCityFormSubmitted = false;
        this.postCityForm.reset();
        //this.loadCities();
      }
    });
  }

  public editClicked(city: City): void {
    this.editCityID = city.cityID;
  }

  public updateClicked(i: number): void {
    this.citiesService.putCity(this.putCityFormArray.controls[i].value).subscribe({
      next: (response: string) => {
        console.log(response);
      },
      error: (error: any) => { console.log(error); },
      complete: () => {
        this.editCityID = null;

        this.putCityFormArray.controls[i].reset(this.putCityFormArray.controls[i].value);
      }
    })
  }

  public deleteClicked(city: City, i: number): void {
    if (confirm(`Are you sure to delete this city ${city.cityName}?`)) {
      this.citiesService.deleteCity(city.cityID).subscribe({
        next: (response: string) => {
          console.log(response);
        },
        error: (error: any) => { console.log(error); },
        complete: () => {
          this.putCityFormArray.removeAt(i);
          this.cities.splice(i,1);
        }
      });
    }
  }

  refreshClicked(): void {
    this.accountService.postGenerateNewToken().subscribe({
      next: (response: any) => {
        localStorage.setItem("token", response.token);
        localStorage.setItem("refreshToken", response.refreshToken);

        this.loadCities();
      },
      error: (error: string) => {
        console.log(error);
      },
      complete: () => { }
    });
  }

}
