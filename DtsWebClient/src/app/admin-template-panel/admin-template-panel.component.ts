import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Template } from '../_models/template'
import { TemplateDataUpdate } from '../_models/templateDataUpdate';
import { TemplateVersions } from '../_models/templateVersion';
import { Version } from '../_models/Version';
import { NgbModal} from '@ng-bootstrap/ng-bootstrap';
import queries from '../../assets/queries.json';
import { Sort } from '@angular/material';
import { UserData } from '../_models/user';


@Component({
  selector: 'app-admin-template-panel',
  templateUrl: './admin-template-panel.component.html',
  styleUrls: ['./admin-template-panel.component.css']
})
export class AdminTemplatePanelComponent implements OnInit {
  closeResult: string;
  searchTemplates: string = '';
  searchVersions: string = '';
  templates: Template[];
  sortedTemplates: Template[];
  templateChosen: boolean;
  pickedTemplate: TemplateVersions;
  sortedVersions: Version[];
  templateContent: HTMLElement;
  headTemplateElements = ['Name', 'Version Count', 'Owner', 'Template State'];
  headVersionsElements = ['Creation Date', 'Creator', 'State'];
  selectedEditor: UserData;
  editors: UserData[];

  constructor(
    private apiClient: HttpClient,
    private modalService: NgbModal
  ) {
  }

  ngOnInit() {
    this.getTemplates();
    this.getEditors();
    this.templateChosen = false;
  }

  getTemplates() {
    this.apiClient.get<Template[]>(queries.templatesPath).subscribe(result => {
      this.templates = result;
      this.sortedTemplates = result;
    }, error => console.error(error));
  }

  getEditors() {
    let query = `${queries.userPath}type/editor`
    this.apiClient.get<UserData[]>(query).subscribe(result => {
      this.editors = result;
    }, error => console.error(error));
  }

  switchSelectedEditor(editor: UserData) {
    this.selectedEditor = editor;
  }

  sortTemplates(sort: Sort) {
    const data = this.sortedTemplates;

    if (!sort.active || sort.direction === '') {
      this.sortedTemplates = data;
      return;
    }
    this.sortedTemplates = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'Name': return this.compare(a.name, b.name, isAsc);
        case 'Version Count': return this.compare(a.versionCount, b.versionCount, isAsc);
        case 'Owner': return this.compare(a.owner.surname, b.owner.surname, isAsc);
        case 'Template State': return this.compare(a.templateState, b.templateState, isAsc);
        default: return 0;
      }
    });
  }

  compare(a: number | string, b: number | string, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  filterTemplates(searchTemplates: string) {
    this.sortedTemplates = this.templates.filter(template =>
      template.name.indexOf(searchTemplates) !== -1
      || template.owner.surname.indexOf(searchTemplates) !== -1
      || template.templateState.indexOf(searchTemplates) !== -1);
  }

  sortVersions(sort: Sort) {
    const data = this.sortedVersions;

    if (!sort.active || sort.direction === '') {
      this.sortedVersions = data;
      return;
    }
    this.sortedVersions = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'Creation Date': return this.compare(a.creationTime, b.creationTime, isAsc);
        case 'Creator': return this.compare(a.creator.surname, b.creator.surname, isAsc);
        case 'State': return this.compare(a.versionState, b.versionState, isAsc);
        default: return 0;
      }
    });
  }

  filterVersions(searchVersions: string) {
    this.sortedVersions = this.pickedTemplate.versions.filter(version =>
      version.creationTime.indexOf(searchVersions) !== -1
      || version.creator.surname.indexOf(searchVersions) !== -1
      || version.versionState.indexOf(searchVersions) !== -1);
  }

  switchState(id: string, state: string, name:string, ownerId: string) {

    if (state == "Inactive") {
      let updateData = new TemplateDataUpdate();
      updateData.id = +id;
      updateData.name = name;
      updateData.ownerId = +ownerId;
      updateData.stateId = 1;
      this.apiClient.put(queries.templatesPath + id, updateData).subscribe(result => {
        this.getTemplates();
      }, error => console.error(error));
    }
    else {
      this.apiClient.delete(queries.templatesPath + id).subscribe(result => {
        this.getTemplates();
      }, error => console.error(error));
    }
  }

  changeTemplateOwner(editor: UserData, templateId: string, name: string) {
    let query = `${queries.templatesPath}${templateId}`
    let DTO = {
      name: name,
      ownerId: editor.id,
      stateId: 2,
    };
    this.apiClient.put(query, DTO).subscribe(result => {
      this.getTemplates();
    }, error => console.error(error));
  }

  loadTemplateVersions(id: string) {
    this.templateChosen = true;
    this.apiClient.get<TemplateVersions>(queries.templatesPath + id).subscribe(result => {
      this.pickedTemplate = result;
      this.sortedVersions = result.versions;
    }, error => console.error(error));
  }

  switchVersionState(id: string, versionState: string) {
    if (versionState == "Active") {
      this.apiClient.delete(queries.templateVersions + id).subscribe(result => {
        this.loadTemplateVersions(this.pickedTemplate.id.toString());
      }, error => console.error(error));
    }
    else {
      this.apiClient.put(queries.templatesPath + this.pickedTemplate.id + "/" + id, "Empty Body").subscribe(result => {
        this.loadTemplateVersions(this.pickedTemplate.id.toString());
      }, error => console.error(error));
    }
  }

  showVersion(event: any, content) {
    this.templateChosen = true;
    let versionIndex = event.path[1].rowIndex - 1;
    let version = this.pickedTemplate.versions[versionIndex].templateVersion;
    this.templateContent = document.createElement("DIV");
    this.templateContent.innerHTML = version;
    this.open(content);
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size:'lg' });
    document.getElementById("version-container").appendChild(this.templateContent);
  }

}

