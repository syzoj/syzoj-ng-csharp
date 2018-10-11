import React, { Component } from 'react'
import { DropdownButton, MenuItem } from 'react-bootstrap'
import config from '../../config'
import Loadable from 'react-loadable'

export class Create extends Component {
    constructor(props) {
        super(props)
        this.state = {choice: null, title: "选择题目类型"}
    }

    setChoice(e, i) {
        this.setState({choice: i, title: config.problemTypes[i].title, loaderComponent: Loadable({
            loader: config.problemTypes[i].createComponent,
            loading: () => <p>加载中...</p>
        })})
    }

    render() {
        if(this.state.loaderComponent) {
            const MyComponent = this.state.loaderComponent
            return [
                <DropdownButton bsStyle="default" title={this.state.title} id="dropdownButton" key="dropdownButton">
                    {config.problemTypes.map((type, i) => <MenuItem key={i} onClick={(e) => this.setChoice(e, i)}>{type.title}</MenuItem>)}
                </DropdownButton>,
                <MyComponent key="loadingComponent" />
            ]
        } else {
            return (
                <DropdownButton bsStyle="default" title={this.state.title} id="dropdownButton" key="dropdownButton">
                    {config.problemTypes.map((type, i) => <MenuItem key={i} onClick={(e) => this.setChoice(e, i)}>{type.title}</MenuItem>)}
                </DropdownButton>
            )
        }
    }
}