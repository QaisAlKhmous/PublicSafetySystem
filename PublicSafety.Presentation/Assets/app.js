var app = angular.module('myApp', ["ngRoute", 'ngTable']);


app.config(function ($routeProvider) {



    $routeProvider
       .when("/items", {
            templateUrl: '/Home/Items',
            controller: 'itemsCtrl'

       })

        .when("/matrices", {
            templateUrl: '/Home/Matrices',
            controller: 'matrixCtrl'

        })
        .when("/employees", {
            templateUrl: '/Home/Employees',
            controller: 'employeeCtrl'

        })
        .when("/addEmployee", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        .when("/editEmployee/:employeeId", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        
       


});



app.run(function ($rootScope, $location) {
    
    $rootScope.toastify = function (msg, type) {
        if (type)
            color = "#4fbe87"
        else
            color = "#FF0000";
        if (typeof Toastify !== "undefined") {
            Toastify({
                text: msg,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "center",
                backgroundColor: color
            }).showToast();
        }
    };


})

app.factory('itemService', function ($http) {
    var baseUrl = '/Item'; // MVC controller base URL

    return {
        getItems: function () {
            
            return $http.get(baseUrl + '/GetItems');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        },
        increaseQuantity: function (id, quantity) {
            return $http.post(baseUrl + '/IncreaseQuantity', { id: id, quantity: quantity });
        }
    };
});



app.factory('matrixService', function ($http) {
    var baseUrl = '/Matrix'; // MVC controller base URL

    return {
        getAllMatrices: function () {

            return $http.get(baseUrl + '/GetAllMatrices');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        }
    };
});

app.factory('disposalService', function ($http) {
    var baseUrl = '/Disposal'; // MVC controller base URL

    return {
        addDisposal: function (disposal) {
            return $http.post(baseUrl + '/AddDisposal', disposal);
        }
    };
});

app.factory('employeeService', function ($http) {
    var baseUrl = '/Employee'; // MVC controller base URL

    return {
        getEmployeeById: function (id) {

            return $http.get(baseUrl + '/GetEmployee?Id=' + id);
        },
        addNewEmployee: function (employee) {
            return $http.post(baseUrl + '/AddNewEmployee' , employee);
        },
        updateEmployee: function (employee) {
            return $http.post(baseUrl + '/UpdateEmployee', employee);
        },
        getAllEmployees: function () {
            return $http.get(baseUrl + '/GetAllEmployees');
        },
        deleteEmployee: function (id) {
            return $http.post(baseUrl + '/ResignEmployee', { Id: id })
        }
    };
});

app.controller('sidebarCtrl', function ($scope) {

    $scope.selectedIndex = 0;



})

app.controller('itemsCtrl', function ($scope, NgTableParams, itemService, disposalService, $rootScope) {

    $scope.items = [];
    $scope.newItem = { Name: '', Description: '', Quantity: 0, AddedBy: 'Admin' }
    $scope.clicked = false;
    $scope.selectedItem = null;  
    $scope.addedQuantity = 0;
    $scope.disposal = {ItemId : null,Quantity: 0,DisposalDate: '',DisposalFormPath : '',CreatedBy: 'Admin',ApprovedBy: "Admin"}

   

    $scope.loadItems = function () {
        itemService.getItems().then(function (response) {
            $scope.items = response.data;
            
            $scope.itemsTableParams.settings({ dataset: $scope.items });
            

        }, function (error) {
            console.error("Error loading items", error);
        });
    };

    $scope.itemsTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );
    $scope.loadItems();

    $scope.addNewItem = function () {
        itemService.addItem($scope.newItem).then(function (response) {
            $scope.loadItems();
            $rootScope.toastify('تم اضافة المادة بنجاح',1);
        }, function (error) {
            console.error("Error loading items", error);
        });
    }

    $scope.confirmDelete = function (ItemId) {
        Swal.fire({
            title: 'هل انت متأكد من حذف المادة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteItem(ItemId);
        });
    }
   

    $scope.deleteItem = function (ItemId) {
        itemService.deleteItem(ItemId).then(function (response) {
            $scope.loadItems();
            $rootScope.toastify("تم حذف مادة بنجاح",1)
        }, function (error) {
            console.error("Error deleting items", error);
        });
    }

    $scope.changeSelectedItem = function (item) {
        $scope.selectedItem = item;
    }

    $scope.increaseQuantity = function () {
        console.log($scope.selectedItem);
        itemService.increaseQuantity($scope.selectedItem.ItemId,$scope.addedQuantity).then(function (response) {
            $scope.loadItems();
            $rootScope.toastify("تم زيادة رصيد مادة بنجاح", 1);
        }, function (error) {
            console.error("Error deleting items", error);
        });
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.disposal = scope.disposal || {};
            scope.disposal.DisposalDate = moment().format('YYYY-MM-DD'); // set default
        });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = '';
            });
        });
    });


    $scope.dispose = function () {
        $scope.disposal.ItemId = $scope.selectedItem.ItemId;
        disposalService.addDisposal($scope.disposal).then(function (response) {
            $scope.loadItems();
            $rootScope.toastify("تم اتلاف مادة بنجاح",1)
        }, function (error) {
            console.error("Error deleting items", error);
        })
    }
})

app.controller('matrixCtrl', function ($scope, NgTableParams, matrixService, $http, $rootScope, itemService) {

    $scope.selectedCategory = null;

    
    $scope.loadCategories = function () {
        $http.get('/Category/GetAllCategories').then((res) => {
            $scope.Categories = res.data;
            console.log(res.data)
        })
    }
    $scope.loadCategories();


   
    $scope.loadItemsByCategory = function (CategoryId) {
        $http.get('/Matrix/GetItemsByCategory?CategoryId=' + CategoryId).then((res) => {

            if (res.data.error === 1)
                return 'لا يوجد مصفوفات لهذه الفئة';
            $scope.matrixItems = res.data;
           
        })
    }

    $scope.selectedCategoryChanged = function () {

        $scope.matrixItems = [];
        if ($scope.selectedCategory) {
            $scope.IsMatrixExistsByCategoryId($scope.selectedCategory.CategoryId);
           
        } else {
            $scope.matrixExists = false;
        }
    };

    $scope.IsMatrixExistsByCategoryId = function (CategoryId) {
        if (CategoryId == null)
            return false;
        $http.get('/Matrix/IsMatrixExistsByCategoryId?CategoryId=' + CategoryId).then((res) => {

            $scope.matrixExists = res.data;
            console.log($scope.matrixExists)
            if ($scope.matrixExists) {
                $scope.loadItemsByCategory($scope.selectedCategory.CategoryId)
            }
        })
    }

    $scope.confirmCreateMatrix = function () {
        Swal.fire({
            title: 'هل انت متأكد من اضافة مصفوفة جديدة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.createMatrix();
        });
    }

    $scope.addMatrixItem = { MatrixId: null, ItemId: null, Quantity: 1, Frequency: 1, CreatedBy: 'Admin' };

    $scope.createMatrix = function () {
        $http.post('/Matrix/AddNewMatrix', { CategoryId: $scope.selectedCategory.CategoryId }).then((res) => {
            $rootScope.toastify('تم اضافة مصفوفة جديدة بنجاح', 1)
            $scope.matrixExists = true;
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }

    $scope.addNewItem = function () {
        $http.post('/Matrix/AddItemInMatrix', { MatrixItem: $scope.addMatrixItem }).then((res) => {
            $rootScope.toastify('تم اضافة مادة جديدة الى المصفوفة بنجاح',1)
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    } 

    $scope.loadAddItem = function () {
        $http.get('/Matrix/GetMatrixByCategory?CategoryId=' + $scope.selectedCategory.CategoryId).then((res) => {
            $scope.addMatrixItem.MatrixId = res.data.MatrixId;
        })
        itemService.getItems().then((res) => {
            $scope.items = res.data;
        })
    }

    
    $scope.updateMatrixItem = { MatrixItemId: null, Frequency: 0, Quantity : 0}
    $scope.loadUpdateMatrixItem = function (matrixItem) {
        $scope.updateMatrixItem.MatrixItemId = matrixItem.MatrixItemId;
        $scope.updateMatrixItem.Frequency = matrixItem.Frequency;
        $scope.updateMatrixItem.Quantity = matrixItem.Quantity;
    }

  

    $scope.confirmDelete = function (matrixItemId) {
        Swal.fire({
            title: 'هل انت متأكد من حذف المادة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteMatrixItem(matrixItemId);
        });
       
    }

    $scope.deleteMatrixItem = function (matrixItemId) {
        $http.post('/Matrix/DeleteMatrixItem', {MatrixItemId: matrixItemId}).then((res) => {
            $rootScope.toastify('تم حذف المادة من المصفوفة بنجاح')
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }

    $scope.updateMatrixItemFun = function () {
        $http.post('/Matrix/UpdateMatrixItem', { MatrixItem: $scope.updateMatrixItem }).then((res) => {
            $rootScope.toastify('تم تعديل المادة بنجاح',1)
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }
  
    $scope.isEdit = false;
})

app.controller('employeeCtrl', function ($scope, NgTableParams, employeeService, $location, itemService, $http, $rootScope) {

    $scope.employees = [];

    $scope.searchBy = 0;

    $scope.getAllEmployees = function () {
        employeeService.getAllEmployees().then(function (response) {
            $scope.employees = response.data;

            $scope.employeesTableParams.settings({ dataset: $scope.employees });
        })
    }

    $scope.getAllEmployees();


    $scope.employeesTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );


    $scope.GoToAddPage = function () {
        $location.path('/addEmployee');
    }

    // Handle Issue Exception

    $scope.issuance = { EmployeeId : null,IssuanceId: null, ItemId: null, Quantity: 1, Type: '', ExceptionReason: '', ExceptionFormPath: '' ,CreatedBy : 'Admin'};

    $scope.loadException = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Exception';
        $scope.msg = 'تم اضافة استثناء مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }

    $scope.loadDamaged = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Damaged';
        $scope.msg = 'تم اضافة بدل تلف مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }


    $scope.loadItems = function () {
        itemService.getItems().then((res) => {
            $scope.items = res.data;
        })
    }

    $scope.issue = function () {
        $http.post('/Issuance/AddNewIssuance', $scope.issuance)
            .then(function (res) {

                if (res.data.success === false) {
                    $rootScope.toastify(res.data.message, 0);
                } else {
                    $rootScope.toastify(res.data.message, 1);

                    $scope.issuance = {
                        EmployeeId: null,
                        IssuanceId: null,
                        ItemId: null,
                        Quantity: 1,
                        Type: '',
                        ExceptionReason: '',
                        ExceptionFormPath: '',
                        CreatedBy: 'Admin'
                    };
                    $scope.exceptionForm.$submitted = false;
                }
            })
            .catch(function (err) {

                $rootScope.toastify(err.data ? err.data.message : 'Something went wrong!', 0);
            });
    }

    $scope.issueException = function () {
        $scope.exceptionForm.$setSubmitted();

        if (!$scope.issuance.ItemId) {

            console.warn("Form invalid");
            return;
        }

        $scope.issue()
    }
    $scope.issueDamage = function () {
        $scope.damageForm.$setSubmitted();

        if (!$scope.issuance.ItemId) {

            console.warn("Form invalid");
            return;
        }

        $scope.issue()
    }

    $scope.confirmDelete = function (EmployeeId) {
        Swal.fire({
            title: 'هل انت متأكد من تغيير حالة الموظف الى مستقيل؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteEmployee(EmployeeId);
        });
    }

    $scope.deleteEmployee = function (EmployeeId) {
        employeeService.deleteEmployee(EmployeeId).then((res) => {
            $rootScope.toastify('تم تغيير حالة الموظف الى مستقيل',1)
        })
    }
   
})
app.directive('arabicOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالحروف العربية والمسافات
                var clean = value.replace(/[^\u0600-\u06FF\s]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});

app.directive('englishOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالأحرف الإنجليزية (A-Z, a-z) والمسافات
                var clean = value.replace(/[^a-zA-Z\s]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});

app.directive('numbersOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالأرقام 0-9
                var clean = value.replace(/[^0-9]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});
app.directive('validEmail', function () {
    var EMAIL_REGEXP = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يتحقق من صحة الإيميل
                if (EMAIL_REGEXP.test(value)) {
                    ngModel.$setValidity('email', true);
                } else {
                    ngModel.$setValidity('email', false);
                }

                return value;
            });
        }
    };
});

app.controller('addEmployeeCtrl', function ($scope, NgTableParams, employeeService, $location, $http, $timeout, $routeParams,$rootScope) {

   

    $scope.GoToListPage = function () {
        $location.path('/employees');
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.employee.EmploymentDate = scope.employee.EmploymentDate || {};
            scope.employee.EmploymentDate = moment().format('YYYY-MM-DD'); // set default
        });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = '';
            });
        });
    });


   

    $scope.loadJobTitles = function () {
        $http.get('/JobTitle/GetAllJobTitles')
            .then((res) => {
                $scope.JobTitles = res.data;
            }).catch((rej) => console.log(rej))
    }


    $scope.loadDepartments = function () {
        $http.get('/Department/GetAllDepartments')
            .then((res) => {
                $scope.Departments = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadSections = function () {
        $http.get('/Section/GetAllSections')
            .then((res) => {
                $scope.Sections = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadCategories = function () {
        $http.get('/Category/GetAllCategories')
            .then((res) => {
                console.log(res);
                $scope.Categories = res.data;
            }).catch((rej) => console.log(rej))
    }


    $scope.loadCategories();
    $scope.loadSections();
    $scope.loadDepartments();
    $scope.loadJobTitles();

    $scope.employee = {
        Name: '', Email: '', Phone: '', IsIntern: false, Notes: '', WorkLocation: 'Amman',
        HealthInsuranceFile: '', DepartmentId: null, SectionId: null, JobTitleId: null, CategoryId: null,
        EmploymentDate: '',Notes: ''
    }

    $scope.updateEmployeeId = $routeParams.employeeId;



    $scope.checkIfUpdate = function () {
        if ($scope.updateEmployeeId) {
            console.log("hi");
            employeeService.getEmployeeById($scope.updateEmployeeId)
                .then((res) => {
                  
                   $scope.employee = res.data;

                })
        }
    }
    $scope.checkIfUpdate();

    $scope.saveEmployee = function () {
        $scope.employeeForm.$setSubmitted();

        if ($scope.employeeForm.$invalid || !$scope.employee.JobTitleId || !$scope.employee.DepartmentId || !$scope.employee.SectionId) {

            console.warn("Form invalid");
            return;
        }


        if ($scope.updateEmployeeId ) {
            employeeService.updateEmployee($scope.employee)
                .then((res) => {
                    $rootScope.toastify('تم تعديل بيانات الموظف بنجاح',1);
                }).catch((rej) => console.log(rej))
        }
        else {
            employeeService.addNewEmployee($scope.employee)
                .then((res) => {
                    $rootScope.toastify('تم اضافة موظف بنجاح', 1);
                    $location.path('/editEmployee/' + res.data.id);
                }).catch((rej) => console.log(rej))

        }

       

       
      
    }

    $scope.downloadHealthInsurance = function (userId) {
        $http.get('/Upload/DownloadHealthInsuranceFile', { params: { userId: userId } })
            .then(function (response) {
                if (response.data.success) {

                    window.open(response.data.fileUrl, '_blank');
                    toastr.success("File is ready for download.");
                } else {
                    toastr.error(response.data.message);
                }
            })
            .catch(function (err) {
                toastr.error("حدث خطأ أثناء تحميل الملف.");
                console.error(err);
            });
    };


    $timeout(function () {
        Dropzone.autoDiscover = false;

        $scope.myDropzone = new Dropzone("#dropzone", {
            paramName: "file",
            addRemoveLinks: true,
            maxFiles: 1,
            acceptedFiles: '.jpeg,.jpg,.png,.gif,.pdf,.doc,.msg',
            dictDefaultMessage: '<strong>Upload a Doc</strong><br/><small>From your computer</small>',
            url: "/Upload/UploadFile",

            init: function () {
                var dz = this;

                this.on("success", function (file, response) {
                    $scope.$applyAsync(function () {
                        $scope.employee.HealthInsuranceFile = '/Uploads/' + response.tempId;
                    });
                });

                // If updating employee, load the existing file now
                if ($scope.employee && $scope.employee.HealthInsuranceFile) {
                    var existingFileName = $scope.employee.HealthInsuranceFile.split('/').pop();
                    var existingFileUrl = $scope.employee.HealthInsuranceFile;

                    var mockFile = { name: existingFileName, size: 12345, accepted: true };

                    dz.emit("addedfile", mockFile);
                    dz.emit("complete", mockFile);

                    // For images, show thumbnail
                    if (/\.(jpg|jpeg|png|gif)$/i.test(mockFile.name)) {
                        dz.emit("thumbnail", mockFile, existingFileUrl);
                    }

                    // Add clickable link to open file
                    mockFile.previewElement.querySelector("a.dz-remove").addEventListener("click", function (e) {
                        window.open(existingFileUrl, "_blank");
                    });

                    dz.files.push(mockFile);
                }
            }
        });
    },200);



  


    
})